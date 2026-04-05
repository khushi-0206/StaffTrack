namespace AuthService.Domain.Entities;

/// <summary>
/// Application user account.
/// </summary>
public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public Guid? ManagerId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFirstLogin { get; set; } = true;

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<Otp> Otps { get; set; } = new List<Otp>();
}
