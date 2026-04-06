using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Infrastructure.Persistence.Repositories;

public class LeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly LeaveDbContext _context;

    public LeaveBalanceRepository(LeaveDbContext context)
    {
        _context = context;
    }

    public Task<LeaveBalance?> GetAsync(Guid employeeId, int leaveTypeId, CancellationToken cancellationToken = default) =>
        _context.LeaveBalances
            .Include(x => x.LeaveType)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId, cancellationToken);

    public async Task<IReadOnlyList<LeaveBalance>> ListByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await _context.LeaveBalances
            .AsNoTracking()
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.LeaveTypeId)
            .ToListAsync(cancellationToken);

    public Task<int> CountByLeaveTypeAsync(int leaveTypeId, CancellationToken cancellationToken = default) =>
        _context.LeaveBalances.CountAsync(x => x.LeaveTypeId == leaveTypeId, cancellationToken);

    public void Add(LeaveBalance balance) => _context.LeaveBalances.Add(balance);

    public void Update(LeaveBalance balance) => _context.LeaveBalances.Update(balance);
}
