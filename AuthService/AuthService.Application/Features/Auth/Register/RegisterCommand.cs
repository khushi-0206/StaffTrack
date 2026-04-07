using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.Register;

public record RegisterCommand(RegisterRequestDto Request) : IRequest<RegisterResponseDto>;
