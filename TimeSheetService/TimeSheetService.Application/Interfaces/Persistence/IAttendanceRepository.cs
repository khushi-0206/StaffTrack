using TimeSheetService.Application.Common.Models;
using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Application.Interfaces.Persistence;

public interface IAttendanceRepository
{
    Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Attendance?> GetByEmployeeAndDateAsync(Guid employeeId, DateOnly date, bool track, CancellationToken cancellationToken = default);
    Task<PagedResult<Attendance>> SearchAsync(Guid? employeeId, DateOnly? from, DateOnly? to, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendance>> ListByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendance>> ListByEmployeeAsync(Guid employeeId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    void Add(Attendance attendance);
    void Update(Attendance attendance);
}
