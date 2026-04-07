using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Infrastructure.Persistence.Repositories;

namespace LeaveService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly LeaveDbContext _context;

    public UnitOfWork(LeaveDbContext context)
    {
        _context = context;
        LeaveRequests = new LeaveRequestRepository(context);
        LeaveBalances = new LeaveBalanceRepository(context);
        LeaveTypes = new LeaveTypeRepository(context);
        LeaveHistories = new LeaveHistoryRepository(context);
    }

    public ILeaveRequestRepository LeaveRequests { get; }
    public ILeaveBalanceRepository LeaveBalances { get; }
    public ILeaveTypeRepository LeaveTypes { get; }
    public ILeaveHistoryRepository LeaveHistories { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
