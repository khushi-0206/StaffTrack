using AuthService.Application.DTOs.Auth;
using AuthService.Application.Features.Auth.ChangePassword;
using AuthService.Application.Features.Auth.ForgotPassword;
using AuthService.Application.Features.Auth.Login;
using AuthService.Application.Features.Auth.Me;
using AuthService.Application.Features.Auth.TokenRefresh;
using AuthService.Application.Features.Auth.Register;
using AuthService.Application.Features.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// First user: creates System Admin with password. Later: Admin/HR creates users (temp password emailed).
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _mediator.Send(new RegisterCommand(request));
        return Ok(result);
    }

    /// <summary>
    /// Returns the current user from the JWT. Used by other services to validate tokens against Auth.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponseDto>> Me()
    {
        var result = await _mediator.Send(new MeQuery());
        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _mediator.Send(new LoginCommand(request));
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _mediator.Send(new TokenRefreshCommand(request));
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        await _mediator.Send(new ChangePasswordCommand(request));
        return Ok(new { message = "Password updated. Please sign in again; existing refresh tokens were revoked." });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        await _mediator.Send(new ForgotPasswordCommand(request));
        return Ok(new { message = "If an account exists for this email, a reset code has been sent." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        await _mediator.Send(new ResetPasswordCommand(request));
        return Ok(new { message = "Password has been reset. You can sign in with your new password." });
    }
}
