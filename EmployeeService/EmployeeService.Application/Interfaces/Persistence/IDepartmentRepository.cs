using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces.Persistence;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);

    void Add(Department department);

    void Update(Department department);

    void Remove(Department department);

    Task<int> CountEmployeesAsync(int departmentId, CancellationToken cancellationToken = default);
}
