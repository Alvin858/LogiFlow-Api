using FluentValidation;
using LogiFlow.Application.DTOs.Vehicles;
using LogiFlow.Application.DTOs.Drivers;

namespace LogiFlow.Application.Validators;

public class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
{
    public CreateVehicleRequestValidator() { RuleFor(x => x.VehicleType).NotEmpty().MaximumLength(100); RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(30); RuleFor(x => x.CapacityKg).GreaterThan(0); RuleFor(x => x.InsuranceExpiryDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).When(x => x.InsuranceExpiryDate.HasValue); RuleFor(x => x.FitnessExpiryDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).When(x => x.FitnessExpiryDate.HasValue); }
}
public class UpdateVehicleRequestValidator : AbstractValidator<UpdateVehicleRequest>
{
    public UpdateVehicleRequestValidator() { RuleFor(x => x.VehicleType).NotEmpty().MaximumLength(100); RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(30); RuleFor(x => x.CapacityKg).GreaterThan(0); }
}
public class UpdateVehicleStatusRequestValidator : AbstractValidator<UpdateVehicleStatusRequest> { public UpdateVehicleStatusRequestValidator() => RuleFor(x => x.Status).IsInEnum(); }
public class CreateDriverRequestValidator : AbstractValidator<CreateDriverRequest>
{
    public CreateDriverRequestValidator() { RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100); RuleFor(x => x.LastName).NotEmpty().MaximumLength(100); RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty().MinimumLength(8).Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]"); RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50); RuleFor(x => x.LicenseExpiryDate).GreaterThan(DateTime.UtcNow.Date); RuleFor(x => x.ExperienceYears).InclusiveBetween(0, 60); }
}
public class UpdateDriverRequestValidator : AbstractValidator<UpdateDriverRequest>
{
    public UpdateDriverRequestValidator() { RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100); RuleFor(x => x.LastName).NotEmpty().MaximumLength(100); RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(50); RuleFor(x => x.LicenseExpiryDate).GreaterThan(DateTime.UtcNow.Date); RuleFor(x => x.ExperienceYears).InclusiveBetween(0, 60); }
}
public class UpdateDriverAvailabilityRequestValidator : AbstractValidator<UpdateDriverAvailabilityRequest> { }
