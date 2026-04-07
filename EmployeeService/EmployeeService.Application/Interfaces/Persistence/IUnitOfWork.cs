namespace EmployeeService.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    IRoleRepository Roles { get; }
    ILeaveTypeRepository LeaveTypes { get; }
    IHolidayRepository Holidays { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
