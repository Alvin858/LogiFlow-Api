using FluentValidation;
using LogiFlow.Application.DTOs.Customers;

namespace LogiFlow.Application.Validators;

public class UpdateCustomerProfileRequestValidator : AbstractValidator<UpdateCustomerProfileRequest>
{
    public UpdateCustomerProfileRequestValidator() { RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200); RuleFor(x => x.ContactPerson).NotEmpty().MaximumLength(200); RuleFor(x => x.TaxNumber).MaximumLength(50); RuleFor(x => x.PhoneNumber).MaximumLength(30); }
}
public class CreateAddressRequestValidator : AbstractValidator<CreateAddressRequest>
{
    public CreateAddressRequestValidator() { RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(250); RuleFor(x => x.AddressLine2).MaximumLength(250); RuleFor(x => x.City).NotEmpty().MaximumLength(100); RuleFor(x => x.State).NotEmpty().MaximumLength(100); RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20); RuleFor(x => x.Country).NotEmpty().MaximumLength(100); }
}
public class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator() { RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(250); RuleFor(x => x.AddressLine2).MaximumLength(250); RuleFor(x => x.City).NotEmpty().MaximumLength(100); RuleFor(x => x.State).NotEmpty().MaximumLength(100); RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20); RuleFor(x => x.Country).NotEmpty().MaximumLength(100); }
}
