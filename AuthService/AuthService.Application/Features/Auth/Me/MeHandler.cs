using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.Auth.Me;

public class MeHandler : IRequestHandler<MeQuery, MeResponseDto>
{
    private readonly IAuthDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MeHandler(IAuthDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<MeResponseDto> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
                     ?? throw new UnauthorizedAppException("Invalid or missing user context.");

        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            throw new UnauthorizedAppException("User no longer exists.");

        return new MeResponseDto(user.Id, user.Email, user.Name, user.Role.Name);
    }
}
