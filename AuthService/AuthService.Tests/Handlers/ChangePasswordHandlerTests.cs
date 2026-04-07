using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.ChangePassword;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Services;
using AuthService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Handlers;

[TestFixture]
public class ChangePasswordHandlerTests
{
    [Test]
    public async Task ChangePassword_Success_UpdatesHash_AndRevokesRefreshTokens()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = await TestUsers.AddAdminAsync(db, hasher, "cp@local.test", "Oldpass1!");

        var rt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.RefreshTokens.Add(rt);
        await db.SaveChangesAsync(CancellationToken.None);

        ICurrentUserService current = TestUsers.MockSystemAdmin(user.Id);
        var handler = new ChangePasswordHandler(db, hasher, current);

        await handler.Handle(new ChangePasswordCommand(new ChangePasswordRequestDto
        {
            CurrentPassword = "Oldpass1!",
            NewPassword = "Newpass1!"
        }), CancellationToken.None);

        var reloaded = await db.Users.FirstAsync(u => u.Id == user.Id);
        Assert.That(hasher.Verify("Newpass1!", reloaded.PasswordHash), Is.True);
        Assert.That(reloaded.IsFirstLogin, Is.False);

        var token = await db.RefreshTokens.FirstAsync(r => r.Id == rt.Id);
        Assert.That(token.IsRevoked, Is.True);
    }

    [Test]
    public void ChangePassword_Throws_WhenCurrentPasswordWrong()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = TestUsers.AddAdminAsync(db, hasher, "w@local.test", "Right1!").GetAwaiter().GetResult();

        ICurrentUserService current = TestUsers.MockSystemAdmin(user.Id);
        var handler = new ChangePasswordHandler(db, hasher, current);

        Assert.ThrowsAsync<AppException>(async () =>
            await handler.Handle(new ChangePasswordCommand(new ChangePasswordRequestDto
            {
                CurrentPassword = "Wrong1!",
                NewPassword = "Newpass1!"
            }), CancellationToken.None));
    }

    [Test]
    public void ChangePassword_ThrowsUnauthorized_WhenNoUserId()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var handler = new ChangePasswordHandler(db, new PasswordHasher(), TestUsers.Anonymous());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new ChangePasswordCommand(new ChangePasswordRequestDto
            {
                CurrentPassword = "a",
                NewPassword = "Newpass1!"
            }), CancellationToken.None));
    }
}
