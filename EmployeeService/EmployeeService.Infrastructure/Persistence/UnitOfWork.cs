using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Infrastructure.Persistence.Repositories;

namespace EmployeeService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly EmployeeDbContext _db;
    private IEmployeeRepository? _employees;
    private IDepartmentRepository? _departments;
    private IRoleRepository? _roles;
    private ILeaveTypeRepository? _leaveTypes;
    private IHolidayRepository? _holidays;

    public UnitOfWork(EmployeeDbContext db)
    {
        _db = db;
    }

    public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_db);

    public IDepartmentRepository Departments => _departments ??= new DepartmentRepository(_db);

    public IRoleRepository Roles => _roles ??= new RoleRepository(_db);

    public ILeaveTypeRepository LeaveTypes => _leaveTypes ??= new LeaveTypeRepository(_db);

    public IHolidayRepository Holidays => _holidays ??= new HolidayRepository(_db);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);

    public void Dispose() => _db.Dispose();
}
