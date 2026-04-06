using LeaveService.Domain.Common;

namespace LeaveService.Domain.Entities;

/// <summary>
/// Leave catalog stored in Leave Service (aligned with EmployeeService leave types for StaffTrack).
/// </summary>
public class LeaveType : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MaxDays { get; set; }

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}
