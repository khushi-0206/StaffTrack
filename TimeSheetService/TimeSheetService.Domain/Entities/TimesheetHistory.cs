using TimeSheetService.Domain.Enums;

namespace TimeSheetService.Domain.Entities;

public class TimesheetHistory
{
    public int Id { get; set; }
    public Guid TimesheetId { get; set; }
    public Timesheet Timesheet { get; set; } = null!;
    public TimesheetHistoryAction Action { get; set; }
    public Guid ActionBy { get; set; }
    public DateTime ActionDate { get; set; }
    public string? Remarks { get; set; }
}
