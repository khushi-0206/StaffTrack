using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Infrastructure.Persistence.Repositories;

public class LeaveTypeRepository : ILeaveTypeRepository
{
    private readonly LeaveDbContext _context;

    public LeaveTypeRepository(LeaveDbContext context)
    {
        _context = context;
    }

    public Task<LeaveType?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.LeaveTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        var n = name.Trim().ToLowerInvariant();
        var q = _context.LeaveTypes.Where(x => x.Name.ToLower() == n);
        if (excludeId is { } id)
            q = q.Where(x => x.Id != id);
        return q.AnyAsync(cancellationToken);
    }

    public void Add(LeaveType leaveType) => _context.LeaveTypes.Add(leaveType);

    public void Update(LeaveType leaveType) => _context.LeaveTypes.Update(leaveType);

    public void Remove(LeaveType leaveType) => _context.LeaveTypes.Remove(leaveType);
}
