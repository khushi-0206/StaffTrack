namespace AuthService.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = null!;
}
