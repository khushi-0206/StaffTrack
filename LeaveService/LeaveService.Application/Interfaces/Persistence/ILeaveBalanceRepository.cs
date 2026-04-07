using LeaveService.Domain.Entities;

namespace LeaveService.Application.Interfaces.Persistence;

public interface ILeaveBalanceRepository
{
    Task<LeaveBalance?> GetAsync(Guid employeeId, int leaveTypeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveBalance>> ListByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<int> CountByLeaveTypeAsync(int leaveTypeId, CancellationToken cancellationToken = default);

    void Add(LeaveBalance balance);

    void Update(LeaveBalance balance);
}
