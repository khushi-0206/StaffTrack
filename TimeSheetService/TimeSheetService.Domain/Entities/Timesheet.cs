using TimeSheetService.Domain.Common;
using TimeSheetService.Domain.Enums;

namespace TimeSheetService.Domain.Entities;

/// <summary>One timesheet per employee per calendar work date.</summary>
public class Timesheet : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalHours { get; set; }
    public TimesheetStatus Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public Guid? ApprovedByEmployeeId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<TimeEntry> Entries { get; set; } = new List<TimeEntry>();
    public ICollection<TimesheetHistory> Histories { get; set; } = new List<TimesheetHistory>();
}
