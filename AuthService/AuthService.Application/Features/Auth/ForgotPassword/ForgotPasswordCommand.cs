using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordRequestDto Request) : IRequest<Unit>;
