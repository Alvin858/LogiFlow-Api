using LogiFlow.Application.DTOs.Billing;

namespace LogiFlow.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyCollection<InvoiceResponse>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyCollection<InvoiceResponse>> GetCustomerInvoicesAsync(int userId, CancellationToken ct);
    Task<InvoiceResponse> GetByIdAsync(int id, int? userId, CancellationToken ct);
    Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken ct);
    Task<InvoiceResponse> UpdateAsync(int id, UpdateInvoiceRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<PaymentResponse>> GetPaymentsAsync(int? userId, CancellationToken ct);
    Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request, int? userId, CancellationToken ct);
    Task<PaymentResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request, CancellationToken ct);
}
