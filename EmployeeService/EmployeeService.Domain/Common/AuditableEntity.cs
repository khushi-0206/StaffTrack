namespace EmployeeService.Domain.Common;

/// <summary>
/// Base type for audit columns maintained by the infrastructure layer.
/// </summary>
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
