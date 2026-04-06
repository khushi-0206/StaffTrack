namespace TimeSheetService.Application.Configuration;

public class AuthServiceOptions
{
    public const string SectionName = "AuthService";
    public string BaseUrl { get; set; } = string.Empty;
    public bool ValidateWithAuthService { get; set; }
}
