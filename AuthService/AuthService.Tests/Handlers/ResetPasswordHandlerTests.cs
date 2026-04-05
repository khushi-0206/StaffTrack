using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.ResetPassword;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Services;
using AuthService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Handlers;

[TestFixture]
public class ResetPasswordHandlerTests
{
    [Test]
    public async Task ResetPassword_Success_WithValidOtp()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = await TestUsers.AddAdminAsync(db, hasher, "rs@local.test", "Old1!");

        var otp = new Otp
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = "123456",
            ExpiryTime = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };
        db.Otps.Add(otp);

        var rt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "tok",
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.RefreshTokens.Add(rt);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new ResetPasswordHandler(db, hasher);
        await handler.Handle(new ResetPasswordCommand(new ResetPasswordRequestDto
        {
            Email = "rs@local.test",
            Code = "123456",
            NewPassword = "Newpass1!"
        }), CancellationToken.None);

        var reloaded = await db.Users.FirstAsync(u => u.Id == user.Id);
        Assert.That(hasher.Verify("Newpass1!", reloaded.PasswordHash), Is.True);
        Assert.That(reloaded.IsFirstLogin, Is.False);
        Assert.That((await db.RefreshTokens.FirstAsync(r => r.Id == rt.Id)).IsRevoked, Is.True);
    }

    [Test]
    public void ResetPassword_Throws_WhenOtpInvalid()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        _ = TestUsers.AddAdminAsync(db, hasher, "bad@local.test", "Old1!").GetAwaiter().GetResult();

        var handler = new ResetPasswordHandler(db, hasher);

        Assert.ThrowsAsync<AppException>(async () =>
            await handler.Handle(new ResetPasswordCommand(new ResetPasswordRequestDto
            {
                Email = "bad@local.test",
                Code = "999999",
                NewPassword = "Newpass1!"
            }), CancellationToken.None));
    }
}
