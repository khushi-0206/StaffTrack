using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces.Persistence;

public interface ILeaveTypeRepository
{
    Task<LeaveType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(LeaveType leaveType);

    void Update(LeaveType leaveType);

    void Remove(LeaveType leaveType);
}
