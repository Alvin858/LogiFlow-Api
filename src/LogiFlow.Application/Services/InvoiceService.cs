using LogiFlow.Application.DTOs.Billing;
using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class InvoiceService(IApplicationDbContext db, INotificationService notifications) : IInvoiceService
{
    public async Task<IReadOnlyCollection<InvoiceResponse>> GetAllAsync(CancellationToken ct) =>
        await db.Invoices.AsNoTracking().OrderByDescending(x => x.IssuedAtUtc).Select(MapExpression()).ToListAsync(ct);

    public async Task<IReadOnlyCollection<InvoiceResponse>> GetCustomerInvoicesAsync(int userId, CancellationToken ct)
    {
        var customer = await db.Customers.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId, ct)
            ?? throw new NotFoundException("Customer profile not found.");
        return await db.Invoices.AsNoTracking().Where(x => x.CustomerId == customer.Id)
            .OrderByDescending(x => x.IssuedAtUtc).Select(MapExpression()).ToListAsync(ct);
    }

    public async Task<InvoiceResponse> GetByIdAsync(int id, int? userId, CancellationToken ct)
    {
        var invoice = await db.Invoices.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Invoice not found.");
        if (userId.HasValue)
        {
            var owns = await db.Customers.AnyAsync(x => x.UserId == userId.Value && x.Id == invoice.CustomerId, ct);
            if (!owns) throw new UnauthorizedException("You cannot access this invoice.");
        }
        return Map(invoice);
    }

    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken ct)
    {
        if (!await db.Shipments.AnyAsync(x => x.Id == request.ShipmentId && x.CustomerId == request.CustomerId, ct))
            throw new BadRequestException("Shipment does not belong to the specified customer.");
        if (!await db.Customers.AnyAsync(x => x.Id == request.CustomerId, ct))
            throw new NotFoundException("Customer not found.");
        if (request.Subtotal < 0 || request.TaxAmount < 0 || request.DiscountAmount < 0)
            throw new BadRequestException("Amounts cannot be negative.");
        if (request.DiscountAmount > request.Subtotal + request.TaxAmount)
            throw new BadRequestException("Discount cannot exceed the invoice amount.");
        if (await db.Invoices.AnyAsync(x => x.ShipmentId == request.ShipmentId, ct))
            throw new ConflictException("An invoice already exists for this shipment.");

        var invoice = new Invoice
        {
            InvoiceNumber = await GenerateInvoiceNumberAsync(ct),
            ShipmentId = request.ShipmentId,
            CustomerId = request.CustomerId,
            Subtotal = request.Subtotal,
            TaxAmount = request.TaxAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = request.Subtotal + request.TaxAmount - request.DiscountAmount,
            Status = "Issued",
            IssuedAtUtc = DateTime.UtcNow,
            DueDateUtc = request.DueDateUtc
        };
        db.Invoices.Add(invoice);
        await db.SaveChangesAsync(ct);
        await NotifyCustomerAsync(invoice.CustomerId, "Invoice generated", $"Invoice {invoice.InvoiceNumber} has been generated.", ct);
        return Map(invoice);
    }

    public async Task<InvoiceResponse> UpdateAsync(int id, UpdateInvoiceRequest request, CancellationToken ct)
    {
        var invoice = await db.Invoices.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("Invoice not found.");
        if (invoice.Status == "Paid") throw new BadRequestException("A paid invoice cannot be updated.");
        if (request.Subtotal < 0 || request.TaxAmount < 0 || request.DiscountAmount < 0) throw new BadRequestException("Amounts cannot be negative.");
        invoice.Subtotal = request.Subtotal;
        invoice.TaxAmount = request.TaxAmount;
        invoice.DiscountAmount = request.DiscountAmount;
        invoice.TotalAmount = request.Subtotal + request.TaxAmount - request.DiscountAmount;
        invoice.Status = request.Status.Trim();
        invoice.DueDateUtc = request.DueDateUtc;
        await db.SaveChangesAsync(ct);
        return Map(invoice);
    }

    public async Task<IReadOnlyCollection<PaymentResponse>> GetPaymentsAsync(int? userId, CancellationToken ct)
    {
        var query = db.Payments.AsNoTracking().AsQueryable();
        if (userId.HasValue)
        {
            var customerId = await db.Customers.Where(x => x.UserId == userId.Value).Select(x => (int?)x.Id).SingleOrDefaultAsync(ct)
                ?? throw new NotFoundException("Customer profile not found.");
            query = query.Where(x => x.Invoice.CustomerId == customerId);
        }
        return await query.OrderByDescending(x => x.CreatedAtUtc).Select(x => new PaymentResponse(x.Id, x.InvoiceId, x.PaymentReference, x.PaymentMethod, x.Amount, x.Status, x.CreatedAtUtc, x.PaidAtUtc)).ToListAsync(ct);
    }

    public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request, int? userId, CancellationToken ct)
    {
        var invoice = await db.Invoices.Include(x => x.Payments).SingleOrDefaultAsync(x => x.Id == request.InvoiceId, ct)
            ?? throw new NotFoundException("Invoice not found.");
        if (userId.HasValue)
        {
            var owns = await db.Customers.AnyAsync(x => x.UserId == userId.Value && x.Id == invoice.CustomerId, ct);
            if (!owns) throw new UnauthorizedException("You cannot pay this invoice.");
        }
        if (invoice.Status == "Paid") throw new BadRequestException("Invoice is already paid.");
        if (request.Amount <= 0) throw new BadRequestException("Payment amount must be greater than zero.");
        var paidTotal = invoice.Payments.Where(x => x.Status == "Completed").Sum(x => x.Amount);
        if (paidTotal + request.Amount > invoice.TotalAmount) throw new BadRequestException("Payment exceeds the invoice balance.");
        if (await db.Payments.AnyAsync(x => x.PaymentReference == request.PaymentReference, ct)) throw new ConflictException("Payment reference already exists.");

        var payment = new Payment
        {
            InvoiceId = invoice.Id,
            PaymentReference = request.PaymentReference.Trim(),
            PaymentMethod = request.PaymentMethod.Trim(),
            Amount = request.Amount,
            Status = "Completed",
            CreatedAtUtc = DateTime.UtcNow,
            PaidAtUtc = DateTime.UtcNow
        };
        db.Payments.Add(payment);
        if (paidTotal + request.Amount >= invoice.TotalAmount)
        {
            invoice.Status = "Paid";
            invoice.PaidAtUtc = DateTime.UtcNow;
        }
        await db.SaveChangesAsync(ct);
        return Map(payment);
    }

    public async Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request, CancellationToken ct)
    {
        var payment = await db.Payments.Include(x => x.Invoice).SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Payment not found.");
        payment.Status = request.Status.Trim();
        payment.PaidAtUtc = payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ? DateTime.UtcNow : null;
        if (payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
        {
            var paid = await db.Payments.Where(x => x.InvoiceId == payment.InvoiceId && x.Id != id && x.Status == "Completed").SumAsync(x => x.Amount, ct) + payment.Amount;
            if (paid >= payment.Invoice.TotalAmount) { payment.Invoice.Status = "Paid"; payment.Invoice.PaidAtUtc = DateTime.UtcNow; }
        }
        await db.SaveChangesAsync(ct);
        return Map(payment);
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct)
    {
        string number;
        do number = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        while (await db.Invoices.AnyAsync(x => x.InvoiceNumber == number, ct));
        return number;
    }

    private async Task NotifyCustomerAsync(int customerId, string title, string message, CancellationToken ct)
    {
        var userId = await db.Customers.Where(x => x.Id == customerId).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
        if (userId.HasValue)
            await notifications.CreateAsync(new CreateNotificationRequest(userId.Value, title, message, "Billing"), ct);
    }

    private static InvoiceResponse Map(Invoice x) => new(x.Id, x.InvoiceNumber, x.ShipmentId, x.CustomerId, x.Subtotal, x.TaxAmount, x.DiscountAmount, x.TotalAmount, x.Status, x.IssuedAtUtc, x.DueDateUtc, x.PaidAtUtc);
    private static System.Linq.Expressions.Expression<Func<Invoice, InvoiceResponse>> MapExpression() => x => new InvoiceResponse(x.Id, x.InvoiceNumber, x.ShipmentId, x.CustomerId, x.Subtotal, x.TaxAmount, x.DiscountAmount, x.TotalAmount, x.Status, x.IssuedAtUtc, x.DueDateUtc, x.PaidAtUtc);
    private static PaymentResponse Map(Payment x) => new(x.Id, x.InvoiceId, x.PaymentReference, x.PaymentMethod, x.Amount, x.Status, x.CreatedAtUtc, x.PaidAtUtc);
}
