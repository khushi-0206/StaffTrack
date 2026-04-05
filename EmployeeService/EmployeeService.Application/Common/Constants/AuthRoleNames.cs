namespace EmployeeService.Application.Common.Constants;

/// <summary>
/// JWT role claim values issued by StaffTrack Auth Service (must stay in sync with Auth seed data).
/// Policies may accept both <see cref="SystemAdmin"/> and <see cref="Admin"/> for flexibility.
/// </summary>
public static class AuthRoleNames
{
    public const string SystemAdmin = "SystemAdmin";
    public const string Admin = "Admin";
    public const string HR = "HR";
    public const string Manager = "Manager";
    public const string Employee = "Employee";
}
