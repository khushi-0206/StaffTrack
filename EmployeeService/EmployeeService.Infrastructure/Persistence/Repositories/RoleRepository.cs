using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly EmployeeDbContext _db;

    public RoleRepository(EmployeeDbContext db)
    {
        _db = db;
    }

    public Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default) =>
        _db.Roles.AnyAsync(
            r => r.Name == name && (!excludeId.HasValue || r.Id != excludeId.Value),
            cancellationToken);

    public void Add(Role role) => _db.Roles.Add(role);
}
