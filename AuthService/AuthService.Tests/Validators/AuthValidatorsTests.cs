using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.ChangePassword;
using AuthService.Application.Features.Auth.ForgotPassword;
using AuthService.Application.Features.Auth.Login;
using AuthService.Application.Features.Auth.Register;
using AuthService.Application.Features.Auth.ResetPassword;
using AuthService.Application.Features.Auth.TokenRefresh;

namespace AuthService.Tests.Validators;

[TestFixture]
public class AuthValidatorsTests
{
    [Test]
    public void RegisterValidator_InvalidEmail_HasError()
    {
        var v = new RegisterValidator();
        var r = v.Validate(new RegisterCommand(new RegisterRequestDto
        {
            Name = "N",
            Email = "not-email",
            Password = "x"
        }));
        Assert.That(r.IsValid, Is.False);
    }

    [Test]
    public void LoginValidator_MissingPassword_HasError()
    {
        var v = new LoginValidator();
        var r = v.Validate(new LoginCommand(new LoginRequestDto { Email = "a@b.c", Password = "" }));
        Assert.That(r.IsValid, Is.False);
    }

    [Test]
    public void ChangePasswordValidator_WeakNewPassword_HasError()
    {
        var v = new ChangePasswordValidator();
        var r = v.Validate(new ChangePasswordCommand(new ChangePasswordRequestDto
        {
            CurrentPassword = "old",
            NewPassword = "weak"
        }));
        Assert.That(r.IsValid, Is.False);
    }

    [Test]
    public void TokenRefreshValidator_EmptyRefresh_HasError()
    {
        var v = new TokenRefreshValidator();
        var r = v.Validate(new TokenRefreshCommand(new RefreshTokenRequestDto
        {
            RefreshToken = "",
            AccessToken = string.Empty
        }));
        Assert.That(r.IsValid, Is.False);
    }

    [Test]
    public void ForgotPasswordValidator_InvalidEmail_HasError()
    {
        var v = new ForgotPasswordValidator();
        var r = v.Validate(new ForgotPasswordCommand(new ForgotPasswordRequestDto { Email = "bad" }));
        Assert.That(r.IsValid, Is.False);
    }

    [Test]
    public void ResetPasswordValidator_InvalidCodeLength_HasError()
    {
        var v = new ResetPasswordValidator();
        var r = v.Validate(new ResetPasswordCommand(new ResetPasswordRequestDto
        {
            Email = "a@b.c",
            Code = "12",
            NewPassword = "Newpass1!"
        }));
        Assert.That(r.IsValid, Is.False);
    }
}
