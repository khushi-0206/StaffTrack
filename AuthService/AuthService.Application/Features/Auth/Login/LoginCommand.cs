using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.Login;

public record LoginCommand(LoginRequestDto Request) : IRequest<LoginResponseDto>;
