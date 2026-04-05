using EmployeeService.Domain.Entities;

namespace EmployeeService.Application.Interfaces.Persistence;

public interface IHolidayRepository
{
    Task<Holiday?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(Holiday holiday);

    void Remove(Holiday holiday);
}
