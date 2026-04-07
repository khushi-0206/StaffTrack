using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(ChangePasswordRequestDto Request) : IRequest<Unit>;
