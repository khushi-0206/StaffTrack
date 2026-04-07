namespace TimeSheetService.Application.Common;

public static class TimesheetPolicy
{
    /// <summary>Maximum billable/work hours allowed per employee per calendar day (all timesheets).</summary>
    public const decimal MaxHoursPerCalendarDay = 24m;
}
