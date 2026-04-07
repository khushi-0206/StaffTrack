namespace AuthService.Application.Common.Constants;

/// <summary>
/// Role names (must match seeded <see cref="AuthService.Domain.Entities.Role"/> rows).
/// </summary>
public static class RoleNames
{
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string HR = "HR";
    public const string SystemAdmin = "SystemAdmin";
}
