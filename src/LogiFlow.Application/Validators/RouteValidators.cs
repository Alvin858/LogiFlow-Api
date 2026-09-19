using FluentValidation;
using LogiFlow.Application.DTOs.Routes;

namespace LogiFlow.Application.Validators;

public class CreateRouteRequestValidator
    : AbstractValidator<CreateRouteRequest>
{
    public CreateRouteRequestValidator()
    {
        RuleFor(x => x.StartLocation)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Destination)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.DistanceKm)
            .GreaterThan(0);

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}

public class UpdateRouteRequestValidator
    : AbstractValidator<UpdateRouteRequest>
{
    public UpdateRouteRequestValidator()
    {
        RuleFor(x => x.StartLocation)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Destination)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.DistanceKm)
            .GreaterThan(0);

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}

public class CreateRouteStopRequestValidator
    : AbstractValidator<CreateRouteStopRequest>
{
    public CreateRouteStopRequestValidator()
    {
        RuleFor(x => x.StopOrder)
            .GreaterThan(0);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}

public class UpdateRouteStopRequestValidator
    : AbstractValidator<UpdateRouteStopRequest>
{
    public UpdateRouteStopRequestValidator()
    {
        RuleFor(x => x.StopOrder)
            .GreaterThan(0);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}