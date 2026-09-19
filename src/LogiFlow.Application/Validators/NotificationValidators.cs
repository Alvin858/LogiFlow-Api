using FluentValidation;
using LogiFlow.Application.DTOs.Notifications;

namespace LogiFlow.Application.Validators;

public class CreateNotificationValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationValidator() { RuleFor(x => x.UserId).GreaterThan(0); RuleFor(x => x.Title).NotEmpty().MaximumLength(200); RuleFor(x => x.Message).NotEmpty().MaximumLength(1000); RuleFor(x => x.Type).NotEmpty().MaximumLength(50); }
}
