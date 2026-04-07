using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using RefreshTokenEntity = AuthService.Domain.Entities.RefreshToken;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _configuration;

    public LoginHandler(
        IAuthDbContext db,
        IPasswordHasher passwordHasher,
        IJwtService jwt,
        IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken);

        if (user is null)
            throw new UnauthorizedAppException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAppException("Account is disabled.");

        var lockoutEnd = user.LockoutEnd;
        if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.UtcNow)
            throw new UnauthorizedAppException($"Account locked until {lockoutEnd:O} UTC.");

        if (!_passwordHasher.Verify(request.Request.Password, user.PasswordHash))
        {
            await ApplyFailedLoginAsync(user, cancellationToken);
            throw new UnauthorizedAppException("Invalid email or password.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;

        var refreshDays = _configuration.GetValue("Jwt:RefreshTokenExpiryDays", 7);
        var accessMinutes = _configuration.GetValue("Jwt:AccessTokenMinutes", 15);

        var refresh = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _jwt.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(refreshDays),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.RefreshTokens.Add(refresh);

        await _db.SaveChangesAsync(cancellationToken);

        var access = _jwt.GenerateAccessToken(user);

        return new LoginResponseDto
        {
            AccessToken = access,
            RefreshToken = refresh.Token,
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(accessMinutes),
            RefreshTokenExpiresAtUtc = refresh.ExpiryDate,
            IsFirstLogin = user.IsFirstLogin,
            Role = user.Role.Name
        };
    }

    private async Task ApplyFailedLoginAsync(User user, CancellationToken cancellationToken)
    {
        var max = _configuration.GetValue("Security:MaxFailedAttempts", 5);
        var lockoutMinutes = _configuration.GetValue("Security:LockoutMinutes", 15);

        user.FailedLoginAttempts++;
        if (user.FailedLoginAttempts >= max)
        {
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            user.FailedLoginAttempts = 0;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
