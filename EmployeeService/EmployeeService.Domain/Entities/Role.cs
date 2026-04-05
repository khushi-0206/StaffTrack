using EmployeeService.Domain.Common;

namespace EmployeeService.Domain.Entities;

/// <summary>
/// Job / access role used for HR classification (Admin, HR, Manager, Employee).
/// </summary>
public class Role : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
