using EmployeeService.Domain.Common;

namespace EmployeeService.Domain.Entities;

/// <summary>
/// Organizational department.
/// </summary>
public class Department : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
