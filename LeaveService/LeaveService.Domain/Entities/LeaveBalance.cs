namespace LeaveService.Domain.Entities;

/// <summary>Per-employee entitlement for a leave type.</summary>
public class LeaveBalance
{
    public int Id { get; set; }

    public Guid EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public LeaveType LeaveType { get; set; } = null!;

    public int TotalDays { get; set; }

    public int UsedDays { get; set; }

    public int RemainingDays { get; set; }

    public DateTime UpdatedAt { get; set; }
}
