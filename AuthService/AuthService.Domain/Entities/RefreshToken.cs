namespace AuthService.Domain.Entities;

/// <summary>
/// Refresh token for JWT rotation and revocation.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string? ReplacedByToken { get; set; }

    public User User { get; set; } = null!;
}
