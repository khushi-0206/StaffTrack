using AuthService.Application.DTOs.Auth;
using MediatR;

namespace AuthService.Application.Features.Auth.Me;

public record MeQuery : IRequest<MeResponseDto>;
