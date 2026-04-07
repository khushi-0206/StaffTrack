using FluentValidation;

namespace AuthService.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
    }
}
