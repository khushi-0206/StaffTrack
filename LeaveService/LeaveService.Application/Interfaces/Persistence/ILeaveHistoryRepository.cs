using LeaveService.Domain.Entities;

namespace LeaveService.Application.Interfaces.Persistence;

public interface ILeaveHistoryRepository
{
    Task<IReadOnlyList<LeaveHistory>> GetByRequestAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);

    void Add(LeaveHistory history);
}
