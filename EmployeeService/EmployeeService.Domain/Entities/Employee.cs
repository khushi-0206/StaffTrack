using EmployeeService.Domain.Common;
using EmployeeService.Domain.Enums;

namespace EmployeeService.Domain.Entities;

/// <summary>
/// Staff member record managed by HR (separate from Auth Service user accounts).
/// </summary>
public class Employee : AuditableEntity
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    /// <summary>Optional direct manager (another employee).</summary>
    public Guid? ManagerId { get; set; }

    public Employee? Manager { get; set; }

    public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

    public DateTime DateOfJoining { get; set; }

    public EmployeeStatus Status { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
}
