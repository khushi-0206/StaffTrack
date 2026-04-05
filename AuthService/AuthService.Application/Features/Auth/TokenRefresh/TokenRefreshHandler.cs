using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.Application.Features.Auth.TokenRefresh;

public class TokenRefreshHandler : IRequestHandler<TokenRefreshCommand, LoginResponseDto>
{
    private readonly IAuthDbContext _db;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _configuration;

    public TokenRefreshHandler(IAuthDbContext db, IJwtService jwt, IConfiguration configuration)
    {
        _db = db;
        _jwt = jwt;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Handle(TokenRefreshCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenValue = request.Request.RefreshToken.Trim();
        var stored = await _db.RefreshTokens
            .Include(r => r.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(r => r.Token == refreshTokenValue, cancellationToken);

        if (stored is null || stored.IsRevoked)
            throw new UnauthorizedAppException("Invalid refresh token.");

        if (stored.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAppException("Refresh token expired.");

        if (!stored.User.IsActive)
            throw new UnauthorizedAppException("Account is disabled.");

        var accessToken = request.Request.AccessToken?.Trim();
        if (!string.IsNullOrEmpty(accessToken))
        {
            var principal = _jwt.GetPrincipalFromExpiredAccessToken(accessToken);
            var sub = principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (sub is null || !Guid.TryParse(sub, out var accessUserId) || accessUserId != stored.UserId)
                throw new UnauthorizedAppException("Access token does not match refresh token.");
        }

        var newRefreshRaw = _jwt.GenerateRefreshToken();
        var refreshDays = _configuration.GetValue("Jwt:RefreshTokenExpiryDays", 7);
        var accessMinutes = _configuration.GetValue("Jwt:AccessTokenMinutes", 15);

        stored.IsRevoked = true;
        stored.ReplacedByToken = newRefreshRaw;

        var newRefresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = stored.UserId,
            Token = newRefreshRaw,
            ExpiryDate = DateTime.UtcNow.AddDays(refreshDays),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.RefreshTokens.Add(newRefresh);

        await _db.SaveChangesAsync(cancellationToken);

        var access = _jwt.GenerateAccessToken(stored.User);

        return new LoginResponseDto
        {
            AccessToken = access,
            RefreshToken = newRefresh.Token,
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(accessMinutes),
            RefreshTokenExpiresAtUtc = newRefresh.ExpiryDate,
            IsFirstLogin = stored.User.IsFirstLogin,
            Role = stored.User.Role.Name
        };
    }
}
