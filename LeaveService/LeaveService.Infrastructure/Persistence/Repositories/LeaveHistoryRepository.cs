using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Infrastructure.Persistence.Repositories;

public class LeaveHistoryRepository : ILeaveHistoryRepository
{
    private readonly LeaveDbContext _context;

    public LeaveHistoryRepository(LeaveDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<LeaveHistory>> GetByRequestAsync(
        Guid leaveRequestId,
        CancellationToken cancellationToken = default) =>
        await _context.LeaveHistories
            .AsNoTracking()
            .Where(x => x.LeaveRequestId == leaveRequestId)
            .OrderByDescending(x => x.ActionDate)
            .ToListAsync(cancellationToken);

    public void Add(LeaveHistory history) => _context.LeaveHistories.Add(history);
}
