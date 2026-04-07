namespace EmployeeService.Application.Configuration;

public class AuthServiceOptions
{
    public const string SectionName = "AuthService";

    /// <summary>Base URL of Auth API (e.g. https://localhost:7123/).</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// When true, selected write operations forward the Authorization header to GET /api/auth/me.
    /// </summary>
    public bool ValidateWithAuthService { get; set; }
}
