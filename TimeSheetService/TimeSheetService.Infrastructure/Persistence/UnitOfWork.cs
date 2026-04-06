using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Infrastructure.Persistence.Repositories;

namespace TimeSheetService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TimeSheetDbContext _context;

    public UnitOfWork(TimeSheetDbContext context)
    {
        _context = context;
        Timesheets = new TimesheetRepository(context);
        TimeEntries = new TimeEntryRepository(context);
        Attendances = new AttendanceRepository(context);
        Projects = new ProjectRepository(context);
        TimesheetHistories = new TimesheetHistoryRepository(context);
    }

    public ITimesheetRepository Timesheets { get; }
    public ITimeEntryRepository TimeEntries { get; }
    public IAttendanceRepository Attendances { get; }
    public IProjectRepository Projects { get; }
    public ITimesheetHistoryRepository TimesheetHistories { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
