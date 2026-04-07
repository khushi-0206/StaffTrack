using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetService.Infrastructure.Persistence.Repositories;

public class TimeEntryRepository : ITimeEntryRepository
{
    private readonly TimeSheetDbContext _context;

    public TimeEntryRepository(TimeSheetDbContext context) => _context = context;

    public Task<TimeEntry?> GetByIdWithTimesheetAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.TimeEntries
            .Include(x => x.Timesheet)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TimeEntry>> ListByTimesheetAsync(
        Guid timesheetId,
        CancellationToken cancellationToken = default) =>
        await _context.TimeEntries.AsNoTracking()
            .Where(x => x.TimesheetId == timesheetId)
            .OrderBy(x => x.WorkDate)
            .ToListAsync(cancellationToken);

    public Task<decimal> SumHoursForTimesheetAsync(Guid timesheetId, CancellationToken cancellationToken = default) =>
        _context.TimeEntries.Where(x => x.TimesheetId == timesheetId).SumAsync(x => x.HoursWorked, cancellationToken);

    public Task<decimal> SumHoursForEmployeeOnWorkDateAsync(
        Guid employeeId,
        DateOnly workDate,
        Guid? excludeTimeEntryId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.TimeEntries
            .Where(e => e.WorkDate == workDate && !e.Timesheet.IsDeleted && e.Timesheet.EmployeeId == employeeId);
        if (excludeTimeEntryId is { } ex)
            q = q.Where(e => e.Id != ex);
        return q.SumAsync(e => e.HoursWorked, cancellationToken);
    }

    public void Add(TimeEntry entry) => _context.TimeEntries.Add(entry);
    public void Update(TimeEntry entry) => _context.TimeEntries.Update(entry);
    public void Remove(TimeEntry entry) => _context.TimeEntries.Remove(entry);
}
