namespace AuthService.Domain.Entities;

/// <summary>
/// One-time code for password reset (sent via email).
/// </summary>
public class Otp
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public bool IsUsed { get; set; }

    public User User { get; set; } = null!;
}
