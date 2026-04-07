using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence.Repositories;

public class LeaveTypeRepository : ILeaveTypeRepository
{
    private readonly EmployeeDbContext _db;

    public LeaveTypeRepository(EmployeeDbContext db)
    {
        _db = db;
    }

    public Task<LeaveType?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.LeaveTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public void Add(LeaveType leaveType) => _db.LeaveTypes.Add(leaveType);

    public void Update(LeaveType leaveType) => _db.LeaveTypes.Update(leaveType);

    public void Remove(LeaveType leaveType) => _db.LeaveTypes.Remove(leaveType);
}
