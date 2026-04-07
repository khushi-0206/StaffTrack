using EmployeeService.Application.Common.Models;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _db;

    public EmployeeRepository(EmployeeDbContext db)
    {
        _db = db;
    }

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Employees
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _db.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => !e.IsDeleted && e.Email == email.ToLowerInvariant(), cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid? excludeEmployeeId, CancellationToken cancellationToken) =>
        _db.Employees.AnyAsync(
            e => !e.IsDeleted && e.Email == email && (!excludeEmployeeId.HasValue || e.Id != excludeEmployeeId.Value),
            cancellationToken);

    public async Task<PagedResult<Employee>> SearchAsync(
        int page,
        int pageSize,
        string? search,
        int? departmentId,
        int? roleId,
        EmployeeStatus? status,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default)
    {
        var q = _db.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(e =>
                e.FirstName.Contains(s) ||
                e.LastName.Contains(s) ||
                e.Email.Contains(s));
        }

        if (departmentId.HasValue)
            q = q.Where(e => e.DepartmentId == departmentId.Value);

        if (roleId.HasValue)
            q = q.Where(e => e.RoleId == roleId.Value);

        if (status.HasValue)
            q = q.Where(e => e.Status == status.Value);

        var total = await q.CountAsync(cancellationToken);

        q = q.Include(e => e.Department).Include(e => e.Role);
        q = ApplySorting(q, sortBy, sortDescending);

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Employee>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken)
    {
        var list = await _db.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.DepartmentId == departmentId)
            .Include(e => e.Department)
            .Include(e => e.Role)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<IReadOnlyList<Employee>> GetByManagerAsync(Guid managerId, CancellationToken cancellationToken)
    {
        var list = await _db.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.ManagerId == managerId)
            .Include(e => e.Department)
            .Include(e => e.Role)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
        return list;
    }

    public void Add(Employee employee) => _db.Employees.Add(employee);

    public void Update(Employee employee) => _db.Employees.Update(employee);

    private static IQueryable<Employee> ApplySorting(IQueryable<Employee> q, string? sortBy, bool desc)
    {
        var key = sortBy?.Trim().ToLowerInvariant() ?? "createdat";
        return key switch
        {
            "firstname" => desc ? q.OrderByDescending(e => e.FirstName) : q.OrderBy(e => e.FirstName),
            "lastname" => desc ? q.OrderByDescending(e => e.LastName) : q.OrderBy(e => e.LastName),
            "email" => desc ? q.OrderByDescending(e => e.Email) : q.OrderBy(e => e.Email),
            "dateofjoining" => desc ? q.OrderByDescending(e => e.DateOfJoining) : q.OrderBy(e => e.DateOfJoining),
            "department" => desc
                ? q.OrderByDescending(e => e.Department.Name)
                : q.OrderBy(e => e.Department.Name),
            _ => desc ? q.OrderByDescending(e => e.CreatedAt) : q.OrderBy(e => e.CreatedAt)
        };
    }
}
