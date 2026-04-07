namespace AuthService.Application.DTOs.Auth;

/// <summary>
/// Bootstrap: Name, Email, Password (first user becomes System Admin).
/// Admin/HR: Name, Email, Role (Employee | Manager | HR); temp password emailed.
/// </summary>
public class RegisterRequestDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    /// <summary>Target role when created by Admin/HR (not used for bootstrap).</summary>
    public string? Role { get; set; }
}
