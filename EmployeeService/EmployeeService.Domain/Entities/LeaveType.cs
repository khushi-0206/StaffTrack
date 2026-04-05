using EmployeeService.Domain.Common;

namespace EmployeeService.Domain.Entities;

public class LeaveType : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MaxDays { get; set; }
}
