namespace AuthService.Application.DTOs.Auth;

/// <summary>
/// Current authenticated user (for clients and inter-service validation).
/// </summary>
public record MeResponseDto(Guid Id, string Email, string Name, string Role);
