namespace LeaveService.Application.Configuration;

public class EmployeeServiceOptions
{
    public const string SectionName = "EmployeeService";

    public string BaseUrl { get; set; } = string.Empty;
}
