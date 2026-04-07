using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.Login;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Services;
using AuthService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Handlers;

[TestFixture]
public class LoginHandlerTests
{
    [Test]
    public async Task Login_Success_ReturnsTokens_AndCreatesRefreshToken()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = await TestUsers.AddAdminAsync(db, hasher, "login@local.test", "Secret1!");
        var jwt = new JwtService(TestConfigurationFactory.Create());
        var config = TestConfigurationFactory.Create();
        var handler = new LoginHandler(db, hasher, jwt, config);

        var result = await handler.Handle(new LoginCommand(new LoginRequestDto
        {
            Email = "login@local.test",
            Password = "Secret1!"
        }), CancellationToken.None);

        Assert.That(result.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(result.RefreshToken, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Role, Is.EqualTo("SystemAdmin"));
        var tokens = await db.RefreshTokens.Where(r => r.UserId == user.Id).ToListAsync();
        Assert.That(tokens, Has.Count.EqualTo(1));
        Assert.That(tokens[0].IsRevoked, Is.False);
    }

    [Test]
    public void Login_Throws_WhenEmailUnknown()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var handler = new LoginHandler(db, new PasswordHasher(), new JwtService(TestConfigurationFactory.Create()),
            TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new LoginCommand(new LoginRequestDto
            {
                Email = "none@local.test",
                Password = "x"
            }), CancellationToken.None));
    }

    [Test]
    public void Login_Throws_WhenPasswordWrong_AndIncrementsFailedAttempts()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        _ = TestUsers.AddAdminAsync(db, hasher, "u@local.test", "Right1!").GetAwaiter().GetResult();

        var handler = new LoginHandler(db, hasher, new JwtService(TestConfigurationFactory.Create()),
            TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new LoginCommand(new LoginRequestDto
            {
                Email = "u@local.test",
                Password = "Wrong1!"
            }), CancellationToken.None));

        var u = db.Users.First(x => x.Email == "u@local.test");
        Assert.That(u.FailedLoginAttempts, Is.GreaterThan(0));
    }

    [Test]
    public void Login_Throws_WhenAccountInactive()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        _ = TestUsers.AddUserAsync(db, hasher, "inactive@local.test", "Secret1!", "SystemAdmin", isActive: false)
            .GetAwaiter().GetResult();

        var handler = new LoginHandler(db, hasher, new JwtService(TestConfigurationFactory.Create()),
            TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new LoginCommand(new LoginRequestDto
            {
                Email = "inactive@local.test",
                Password = "Secret1!"
            }), CancellationToken.None));
    }

    [Test]
    public void Login_Throws_WhenLockedOut()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var user = TestUsers.AddAdminAsync(db, hasher, "locked@local.test", "Secret1!").GetAwaiter().GetResult();
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
        db.Users.Update(user);
        db.SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();

        var handler = new LoginHandler(db, hasher, new JwtService(TestConfigurationFactory.Create()),
            TestConfigurationFactory.Create());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new LoginCommand(new LoginRequestDto
            {
                Email = "locked@local.test",
                Password = "Secret1!"
            }), CancellationToken.None));
    }

    [Test]
    public void Login_SetsLockout_AfterMaxFailedAttempts()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        _ = TestUsers.AddAdminAsync(db, hasher, "lockme@local.test", "Secret1!").GetAwaiter().GetResult();

        var config = TestConfigurationFactory.Create(maxFailedAttempts: 2, lockoutMinutes: 30);
        var handler = new LoginHandler(db, hasher, new JwtService(config), config);

        for (var i = 0; i < 2; i++)
        {
            Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
                await handler.Handle(new LoginCommand(new LoginRequestDto
                {
                    Email = "lockme@local.test",
                    Password = "bad"
                }), CancellationToken.None));
        }

        var u = db.Users.First(x => x.Email == "lockme@local.test");
        Assert.That(u.LockoutEnd, Is.Not.Null);
        Assert.That(u.LockoutEnd!.Value, Is.GreaterThan(DateTime.UtcNow));
    }
}
