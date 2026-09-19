using FluentValidation;
using LogiFlow.Application.DTOs.Billing;

namespace LogiFlow.Application.Validators;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceValidator() { RuleFor(x => x.ShipmentId).GreaterThan(0); RuleFor(x => x.CustomerId).GreaterThan(0); RuleFor(x => x.Subtotal).GreaterThanOrEqualTo(0); RuleFor(x => x.TaxAmount).GreaterThanOrEqualTo(0); RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0); }
}
public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceValidator() { RuleFor(x => x.Subtotal).GreaterThanOrEqualTo(0); RuleFor(x => x.TaxAmount).GreaterThanOrEqualTo(0); RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0); RuleFor(x => x.Status).NotEmpty().MaximumLength(30); }
}
public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentValidator() { RuleFor(x => x.InvoiceId).GreaterThan(0); RuleFor(x => x.PaymentReference).NotEmpty().MaximumLength(100); RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50); RuleFor(x => x.Amount).GreaterThan(0); }
}
