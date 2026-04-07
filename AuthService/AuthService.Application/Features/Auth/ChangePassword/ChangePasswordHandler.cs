using AuthService.Application.Common.Exceptions;
using AuthService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IAuthDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordHandler(
        IAuthDbContext db,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new UnauthorizedAppException();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!_passwordHasher.Verify(request.Request.CurrentPassword, user.PasswordHash))
            throw new AppException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(request.Request.NewPassword);
        user.IsFirstLogin = false;

        var refreshTokens = await _db.RefreshTokens
            .Where(r => r.UserId == user.Id && !r.IsRevoked && r.ExpiryDate > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        foreach (var rt in refreshTokens)
            rt.IsRevoked = true;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
