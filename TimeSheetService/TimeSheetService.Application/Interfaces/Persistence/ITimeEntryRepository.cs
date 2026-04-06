using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Application.Interfaces.Persistence;

public interface ITimeEntryRepository
{
    Task<TimeEntry?> GetByIdWithTimesheetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimeEntry>> ListByTimesheetAsync(Guid timesheetId, CancellationToken cancellationToken = default);
    Task<decimal> SumHoursForTimesheetAsync(Guid timesheetId, CancellationToken cancellationToken = default);
    Task<decimal> SumHoursForEmployeeOnWorkDateAsync(Guid employeeId, DateOnly workDate, Guid? excludeTimeEntryId, CancellationToken cancellationToken = default);
    void Add(TimeEntry entry);
    void Update(TimeEntry entry);
    void Remove(TimeEntry entry);
}
