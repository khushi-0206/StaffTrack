using TimeSheetService.Domain.Enums;

namespace TimeSheetService.Domain.Entities;

public class Attendance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal? TotalHours { get; set; }
    public AttendanceStatus Status { get; set; }
    public bool IsDeleted { get; set; }
}
