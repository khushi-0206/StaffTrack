using EmployeeService.Domain.Common;

namespace EmployeeService.Domain.Entities;

public class Holiday : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly Date { get; set; }
}
