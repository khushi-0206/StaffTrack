namespace TimeSheetService.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ITimesheetRepository Timesheets { get; }
    ITimeEntryRepository TimeEntries { get; }
    IAttendanceRepository Attendances { get; }
    IProjectRepository Projects { get; }
    ITimesheetHistoryRepository TimesheetHistories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
