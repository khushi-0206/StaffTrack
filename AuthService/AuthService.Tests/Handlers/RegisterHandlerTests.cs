using AuthService.Application.Common.Constants;
using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.Register;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthService.Tests.Handlers;

[TestFixture]
public class RegisterHandlerTests
{
    [Test]
    public async Task Bootstrap_FirstUser_CreatesSystemAdmin()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        var emailMock = new Mock<IEmailService>(MockBehavior.Strict);
        ICurrentUserService current = TestUsers.Anonymous();

        var handler = new RegisterHandler(db, hasher, emailMock.Object, current);

        var result = await handler.Handle(new RegisterCommand(new RegisterRequestDto
        {
            Name = "Admin",
            Email = "admin@local.test",
            Password = "Bootstrap1!"
        }), CancellationToken.None);

        Assert.That(result.UserId, Is.Not.EqualTo(Guid.Empty));
        var user = await ctx.Users.Include(u => u.Role).FirstAsync(u => u.Id == result.UserId);
        Assert.That(user.Role.Name, Is.EqualTo(RoleNames.SystemAdmin));
        Assert.That(user.IsFirstLogin, Is.True);
        emailMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void Bootstrap_Throws_WhenPasswordMissing()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var handler = new RegisterHandler(db, new PasswordHasher(), Mock.Of<IEmailService>(), TestUsers.Anonymous());

        Assert.ThrowsAsync<AppException>(async () =>
            await handler.Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "A",
                Email = "a@b.c",
                Password = null
            }), CancellationToken.None));
    }

    [Test]
    public void Register_Throws_WhenEmailAlreadyExists()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();

        var bootstrap = new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), TestUsers.Anonymous());
        bootstrap.Handle(new RegisterCommand(new RegisterRequestDto
        {
            Name = "Admin",
            Email = "dup@local.test",
            Password = "Bootstrap1!"
        }), CancellationToken.None).GetAwaiter().GetResult();

        var handler = new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), TestUsers.Anonymous());

        Assert.ThrowsAsync<AppException>(async () =>
            await handler.Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "Other",
                Email = "dup@local.test",
                Password = "Other1!"
            }), CancellationToken.None));
    }

    [Test]
    public void Register_ThrowsUnauthorized_WhenUsersExist_ButCallerNotAuthenticated()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), TestUsers.Anonymous())
            .Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "Admin",
                Email = "a@local.test",
                Password = "Bootstrap1!"
            }), CancellationToken.None).GetAwaiter().GetResult();

        var handler = new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), TestUsers.Anonymous());

        Assert.ThrowsAsync<UnauthorizedAppException>(async () =>
            await handler.Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "U",
                Email = "new@local.test",
                Role = RoleNames.Employee
            }), CancellationToken.None));
    }

    [Test]
    public void Register_ThrowsForbidden_WhenCallerIsEmployee()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();

        new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), TestUsers.Anonymous())
            .Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "Admin",
                Email = "admin@local.test",
                Password = "Bootstrap1!"
            }), CancellationToken.None).GetAwaiter().GetResult();

        var admin = db.Users.First(u => u.Email == "admin@local.test");
        ICurrentUserService current = TestUsers.MockEmployeeCaller(admin.Id);
        var handler = new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), current);

        Assert.ThrowsAsync<ForbiddenAppException>(async () =>
            await handler.Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "U",
                Email = "emp@local.test",
                Role = RoleNames.Employee
            }), CancellationToken.None));
    }

    [Test]
    public async Task RegisterByAdmin_AsSystemAdmin_CreatesUser_AndSendsEmail()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();

        await new RegisterHandler(db, hasher, Mock.Of<IEmailService>(), TestUsers.Anonymous())
            .Handle(new RegisterCommand(new RegisterRequestDto
            {
                Name = "Admin",
                Email = "admin@local.test",
                Password = "Bootstrap1!"
            }), CancellationToken.None);

        var admin = db.Users.First(u => u.Email == "admin@local.test");
        var emailMock = new Mock<IEmailService>();
        emailMock
            .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        ICurrentUserService current = TestUsers.MockSystemAdmin(admin.Id);
        var handler = new RegisterHandler(db, hasher, emailMock.Object, current);

        var result = await handler.Handle(new RegisterCommand(new RegisterRequestDto
        {
            Name = "Employee One",
            Email = "e1@local.test",
            Role = RoleNames.Employee
        }), CancellationToken.None);

        Assert.That(result.UserId, Is.Not.EqualTo(Guid.Empty));
        var created = await ctx.Users.Include(u => u.Role).FirstAsync(u => u.Email == "e1@local.test");
        Assert.That(created.Role.Name, Is.EqualTo(RoleNames.Employee));
        emailMock.Verify(
            e => e.SendEmailAsync("e1@local.test", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
