using FluentValidation;
using LogiFlow.Application.DTOs.Auth;

namespace LogiFlow.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]");
        RuleFor(x => x.PhoneNumber).MaximumLength(30).When(x => x.PhoneNumber is not null);
        RuleFor(x => x.CompanyName).MaximumLength(200).When(x => x.CompanyName is not null);
    }
}
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty(); }
}
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest> { public RefreshTokenRequestValidator() => RuleFor(x => x.RefreshToken).NotEmpty(); }
public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest> { public ForgotPasswordRequestValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress(); }
public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.ResetToken).NotEmpty(); RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]"); }
}
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator() { RuleFor(x => x.CurrentPassword).NotEmpty(); RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]"); }
}
