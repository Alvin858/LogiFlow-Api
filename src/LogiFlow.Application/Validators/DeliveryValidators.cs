using FluentValidation;
using LogiFlow.Application.DTOs.DeliveryDtos;

namespace LogiFlow.Application.Validators;

public class CreateDeliveryRequestValidator
    : AbstractValidator<CreateDeliveryRequest>
{
    public CreateDeliveryRequestValidator()
    {
        RuleFor(x => x.ShipmentId)
            .GreaterThan(0);

        RuleFor(x => x.DriverId)
            .GreaterThan(0);

        RuleFor(x => x.VehicleId)
            .GreaterThan(0);

        RuleFor(x => x.RouteId)
            .GreaterThan(0);
    }
}

public class UpdateDeliveryRequestValidator
    : AbstractValidator<UpdateDeliveryRequest>
{
    public UpdateDeliveryRequestValidator()
    {
        RuleFor(x => x.ShipmentId)
            .GreaterThan(0);

        RuleFor(x => x.DriverId)
            .GreaterThan(0);

        RuleFor(x => x.VehicleId)
            .GreaterThan(0);

        RuleFor(x => x.RouteId)
            .GreaterThan(0);
    }
}

public class FailDeliveryRequestValidator
    : AbstractValidator<FailDeliveryRequest>
{
    public FailDeliveryRequestValidator()
    {
        RuleFor(x => x.FailureReason)
            .NotEmpty()
            .MaximumLength(1000);
    }
}

public class CreateProofOfDeliveryRequestValidator
    : AbstractValidator<CreateProofOfDeliveryRequest>
{
    public CreateProofOfDeliveryRequestValidator()
    {
        RuleFor(x => x.ReceiverName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SignaturePath)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.PhotoPath)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Remarks)
            .MaximumLength(1000);
    }
}