using EmployeeService.Application.Common.Models;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;

namespace EmployeeService.Application.Interfaces.Persistence;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, Guid? excludeEmployeeId, CancellationToken cancellationToken = default);

    Task<PagedResult<Employee>> SearchAsync(
        int page,
        int pageSize,
        string? search,
        int? departmentId,
        int? roleId,
        EmployeeStatus? status,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetByManagerAsync(Guid managerId, CancellationToken cancellationToken = default);

    void Add(Employee employee);

    void Update(Employee employee);
}
