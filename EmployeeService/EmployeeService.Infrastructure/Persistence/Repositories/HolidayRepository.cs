using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly EmployeeDbContext _db;

    public HolidayRepository(EmployeeDbContext db)
    {
        _db = db;
    }

    public Task<Holiday?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Holidays.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Holidays.AsNoTracking().OrderBy(x => x.Date).ToListAsync(cancellationToken);

    public void Add(Holiday holiday) => _db.Holidays.Add(holiday);

    public void Remove(Holiday holiday) => _db.Holidays.Remove(holiday);
}
