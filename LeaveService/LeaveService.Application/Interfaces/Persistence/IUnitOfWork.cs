namespace LeaveService.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ILeaveRequestRepository LeaveRequests { get; }
    ILeaveBalanceRepository LeaveBalances { get; }
    ILeaveTypeRepository LeaveTypes { get; }
    ILeaveHistoryRepository LeaveHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
