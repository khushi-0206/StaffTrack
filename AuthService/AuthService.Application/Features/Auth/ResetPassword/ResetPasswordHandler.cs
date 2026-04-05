using AuthService.Application.Common.Exceptions;
using AuthService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IAuthDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordHandler(IAuthDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken)
                   ?? throw new AppException("Invalid email or code.");

        var code = request.Request.Code.Trim();
        var now = DateTime.UtcNow;

        var otp = await _db.Otps
            .Where(o => o.UserId == user.Id && !o.IsUsed && o.Code == code && o.ExpiryTime > now)
            .OrderByDescending(o => o.ExpiryTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null)
            throw new AppException("Invalid email or code.");

        otp.IsUsed = true;
        user.PasswordHash = _passwordHasher.Hash(request.Request.NewPassword);
        user.IsFirstLogin = false;

        var sessions = await _db.RefreshTokens
            .Where(r => r.UserId == user.Id && !r.IsRevoked)
            .ToListAsync(cancellationToken);
        foreach (var rt in sessions)
            rt.IsRevoked = true;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
