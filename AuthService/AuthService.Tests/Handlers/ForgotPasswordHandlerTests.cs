using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.ForgotPassword;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthService.Tests.Handlers;

[TestFixture]
public class ForgotPasswordHandlerTests
{
    [Test]
    public async Task ForgotPassword_WhenUserExists_CreatesOtp_AndSendsEmail()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;
        var hasher = new PasswordHasher();
        await TestUsers.AddAdminAsync(db, hasher, "fp@local.test", "Secret1!");

        var emailMock = new Mock<IEmailService>();
        emailMock
            .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new ForgotPasswordHandler(db, emailMock.Object, TestConfigurationFactory.Create());

        await handler.Handle(new ForgotPasswordCommand(new ForgotPasswordRequestDto
        {
            Email = "fp@local.test"
        }), CancellationToken.None);

        var otps = await db.Otps.ToListAsync();
        Assert.That(otps, Has.Count.EqualTo(1));
        emailMock.Verify(
            e => e.SendEmailAsync("fp@local.test", "Password reset code", It.Is<string>(b => b.Contains(otps[0].Code)), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ForgotPassword_WhenUserMissing_DoesNotThrow_AndDoesNotSendEmail()
    {
        using var ctx = TestAuthDbContextFactory.Create();
        IAuthDbContext db = ctx;

        var emailMock = new Mock<IEmailService>(MockBehavior.Strict);
        var handler = new ForgotPasswordHandler(db, emailMock.Object, TestConfigurationFactory.Create());

        await handler.Handle(new ForgotPasswordCommand(new ForgotPasswordRequestDto
        {
            Email = "none@local.test"
        }), CancellationToken.None);

        Assert.That(await db.Otps.CountAsync(), Is.Zero);
    }
}
