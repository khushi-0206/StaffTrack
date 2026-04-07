using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequestDto Request) : IRequest<Unit>;
