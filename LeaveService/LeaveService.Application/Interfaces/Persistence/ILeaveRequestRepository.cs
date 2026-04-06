using LeaveService.Application.Common.Models;
using LeaveService.Domain.Entities;
using LeaveService.Domain.Enums;

namespace LeaveService.Application.Interfaces.Persistence;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeaveRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<LeaveRequest>> SearchAsync(
        int page,
        int pageSize,
        Guid? employeeId,
        LeaveRequestStatus? status,
        int? leaveTypeId,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdsAsync(
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingAsync(
        Guid employeeId,
        DateOnly start,
        DateOnly end,
        LeaveRequestStatus[] blockingStatuses,
        Guid? excludeRequestId,
        CancellationToken cancellationToken = default);

    Task<int> CountByLeaveTypeAsync(int leaveTypeId, CancellationToken cancellationToken = default);

    void Add(LeaveRequest request);

    void Update(LeaveRequest request);
}
