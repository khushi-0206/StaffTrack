using FluentValidation;

namespace AuthService.Application.Features.Auth.TokenRefresh;

public class TokenRefreshValidator : AbstractValidator<TokenRefreshCommand>
{
    public TokenRefreshValidator()
    {
        RuleFor(x => x.Request.RefreshToken).NotEmpty();
    }
}
