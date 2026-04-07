using LeaveService.Domain.Enums;

namespace LeaveService.Domain.Entities;

/// <summary>
/// References <see cref="EmployeeId"/> from Employee Service (same Guid as employee record).
/// </summary>
public class LeaveRequest
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public LeaveType LeaveType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Reason { get; set; } = null!;

    public LeaveRequestStatus Status { get; set; }

    /// <summary>Employee Id (from Employee Service) of the approving manager, when approved.</summary>
    public Guid? ApprovedByEmployeeId { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<LeaveHistory> Histories { get; set; } = new List<LeaveHistory>();
}
