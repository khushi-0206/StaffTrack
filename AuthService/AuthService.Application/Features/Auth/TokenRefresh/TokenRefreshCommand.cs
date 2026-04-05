using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.TokenRefresh;

public record TokenRefreshCommand(RefreshTokenRequestDto Request) : IRequest<LoginResponseDto>;
