namespace LogiFlow.Application.DTOs.Billing;

public record CreateInvoiceRequest(
    int ShipmentId,
    int CustomerId,
    decimal Subtotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    DateTime? DueDateUtc);

public record UpdateInvoiceRequest(
    decimal Subtotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    string Status,
    DateTime? DueDateUtc);

public record InvoiceResponse(
    int Id,
    string InvoiceNumber,
    int ShipmentId,
    int CustomerId,
    decimal Subtotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    string Status,
    DateTime IssuedAtUtc,
    DateTime? DueDateUtc,
    DateTime? PaidAtUtc);

public record CreatePaymentRequest(
    int InvoiceId,
    string PaymentReference,
    string PaymentMethod,
    decimal Amount);

public record PaymentResponse(
    int Id,
    int InvoiceId,
    string PaymentReference,
    string PaymentMethod,
    decimal Amount,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);

public record UpdatePaymentStatusRequest(string Status);
