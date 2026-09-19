using FluentValidation;
using LogiFlow.Application.DTOs.Schedules;

namespace LogiFlow.Application.Validators;

public class CreateScheduleRequestValidator
    : AbstractValidator<CreateScheduleRequest>
{
    public CreateScheduleRequestValidator()
    {
        RuleFor(x => x.ShipmentId)
            .GreaterThan(0);

        RuleFor(x => x.DriverId)
            .GreaterThan(0);

        RuleFor(x => x.VehicleId)
            .GreaterThan(0);

        RuleFor(x => x.StartTimeUtc)
            .NotEmpty();

        RuleFor(x => x.EndTimeUtc)
            .NotEmpty()
            .GreaterThan(x => x.StartTimeUtc);
    }
}

public class UpdateScheduleRequestValidator
    : AbstractValidator<UpdateScheduleRequest>
{
    public UpdateScheduleRequestValidator()
    {
        RuleFor(x => x.ShipmentId)
            .GreaterThan(0);

        RuleFor(x => x.DriverId)
            .GreaterThan(0);

        RuleFor(x => x.VehicleId)
            .GreaterThan(0);

        RuleFor(x => x.StartTimeUtc)
            .NotEmpty();

        RuleFor(x => x.EndTimeUtc)
            .NotEmpty()
            .GreaterThan(x => x.StartTimeUtc);
    }
}

public class RescheduleRequestValidator
    : AbstractValidator<RescheduleRequest>
{
    public RescheduleRequestValidator()
    {
        RuleFor(x => x.StartTimeUtc)
            .NotEmpty();

        RuleFor(x => x.EndTimeUtc)
            .NotEmpty()
            .GreaterThan(x => x.StartTimeUtc);
    }
}