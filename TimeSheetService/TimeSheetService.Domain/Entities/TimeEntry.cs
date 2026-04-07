namespace TimeSheetService.Domain.Entities;

public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid TimesheetId { get; set; }
    public Timesheet Timesheet { get; set; } = null!;

    public int? ProjectId { get; set; }
    public Project? Project { get; set; }

    public string ProjectName { get; set; } = null!;
    public string TaskDescription { get; set; } = null!;
    public decimal HoursWorked { get; set; }
    public DateOnly WorkDate { get; set; }
}
