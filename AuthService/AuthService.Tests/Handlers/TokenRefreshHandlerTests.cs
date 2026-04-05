using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.TokenRefresh;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Services;
using AuthService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Handlers;

[TestFixture]
public class TokenRefreshHandlerTests
{
    [Test]
    public async Task Refresh_RotatesToken_AndRevokesOld()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = await TestUsers.AddAdminAsync(db, hasher, "rt@local.test", "Secret1!");
        var jwt = new JwtService(TestConfigurationFactory.Create());
        var config = TestConfigurationFactory.Create();

        var oldRt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = jwt.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.RefreshTokens.Add(oldRt);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new TokenRefreshHandler(db, jwt, config);
        var result = await handler.Handle(new TokenRefreshCommand(new RefreshTokenRequestDto
        {
            RefreshToken = oldRt.Token,
            AccessToken = string.Empty
        }), CancellationToken.None);

        Assert.That(result.RefreshToken, Is.Not.EqualTo(oldRt.Token));
        var reloaded = await db.RefreshTokens.FirstAsync(r => r.Id == oldRt.Id);
        Assert.That(reloaded.IsRevoked, Is.True);
        Assert.That(reloaded.ReplacedByToken, Is.Not.Null);
    }

    [Test]
    public void Refresh_Throws_WhenTokenUnknown()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var jwt = new JwtService(TestConfigurationFactory.Create());
        var handler = new TokenRefreshHandler(db, jwt, TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new TokenRefreshCommand(new RefreshTokenRequestDto
            {
                RefreshToken = "not-in-db",
                AccessToken = string.Empty
            }), CancellationToken.None));
    }

    [Test]
    public void Refresh_Throws_WhenTokenExpired()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = TestUsers.AddAdminAsync(db, hasher, "exp@local.test", "Secret1!").GetAwaiter().GetResult();
        var jwt = new JwtService(TestConfigurationFactory.Create());

        var rt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = jwt.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-8)
        };
        db.RefreshTokens.Add(rt);
        db.SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();

        var handler = new TokenRefreshHandler(db, jwt, TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new TokenRefreshCommand(new RefreshTokenRequestDto
            {
                RefreshToken = rt.Token,
                AccessToken = string.Empty
            }), CancellationToken.None));
    }

    [Test]
    public void Refresh_Throws_WhenAccessTokenUserMismatch()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var u1 = TestUsers.AddAdminAsync(db, hasher, "a@local.test", "Secret1!").GetAwaiter().GetResult();
        var u2 = TestUsers.AddUserAsync(db, hasher, "b@local.test", "Secret1!", "Employee").GetAwaiter().GetResult();

        var jwt = new JwtService(TestConfigurationFactory.Create());
        u1 = db.Users.Include(x => x.Role).First(x => x.Id == u1.Id);
        u2 = db.Users.Include(x => x.Role).First(x => x.Id == u2.Id);

        var accessForU2 = jwt.GenerateAccessToken(u2);

        var rt = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = u1.Id,
            Token = jwt.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.RefreshTokens.Add(rt);
        db.SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();

        var handler = new TokenRefreshHandler(db, jwt, TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new TokenRefreshCommand(new RefreshTokenRequestDto
            {
                RefreshToken = rt.Token,
                AccessToken = accessForU2
            }), CancellationToken.None));
    }
}
