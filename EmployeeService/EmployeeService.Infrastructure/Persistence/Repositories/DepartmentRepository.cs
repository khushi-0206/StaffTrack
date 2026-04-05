using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly EmployeeDbContext _db;

    public DepartmentRepository(EmployeeDbContext db)
    {
        _db = db;
    }

    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default) =>
        _db.Departments.AnyAsync(
            d => d.Name == name && (!excludeId.HasValue || d.Id != excludeId.Value),
            cancellationToken);

    public void Add(Department department) => _db.Departments.Add(department);

    public void Update(Department department) => _db.Departments.Update(department);

    public void Remove(Department department) => _db.Departments.Remove(department);

    public Task<int> CountEmployeesAsync(int departmentId, CancellationToken cancellationToken = default) =>
        _db.Employees.CountAsync(e => !e.IsDeleted && e.DepartmentId == departmentId, cancellationToken);
}
