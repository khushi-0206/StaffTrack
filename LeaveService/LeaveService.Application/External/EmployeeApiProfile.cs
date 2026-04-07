namespace LeaveService.Application.External;

/// <summary>Subset of Employee API payload used by Leave Service.</summary>
public class EmployeeApiProfile
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Guid? ManagerId { get; set; }
}
