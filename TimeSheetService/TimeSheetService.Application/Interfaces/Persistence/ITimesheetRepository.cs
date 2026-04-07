using TimeSheetService.Application.Common.Models;
using TimeSheetService.Domain.Entities;
using TimeSheetService.Domain.Enums;

namespace TimeSheetService.Application.Interfaces.Persistence;

public interface ITimesheetRepository
{
    Task<Timesheet?> GetByIdAsync(Guid id, bool track, CancellationToken cancellationToken = default);
    Task<Timesheet?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveForEmployeeDateAsync(Guid employeeId, DateOnly date, Guid? excludeTimesheetId, CancellationToken cancellationToken = default);
    Task<PagedResult<Timesheet>> SearchAsync(int page, int pageSize, Guid? employeeId, TimesheetStatus? status, DateOnly? from, DateOnly? to, string? sortBy, bool sortDescending, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Timesheet>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Timesheet>> GetByEmployeeIdsAsync(IReadOnlyCollection<Guid> employeeIds, CancellationToken cancellationToken = default);
    void Add(Timesheet timesheet);
    void Update(Timesheet timesheet);
}
