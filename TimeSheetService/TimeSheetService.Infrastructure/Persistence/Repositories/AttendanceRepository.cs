using TimeSheetService.Application.Common.Models;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetService.Infrastructure.Persistence.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly TimeSheetDbContext _context;

    public AttendanceRepository(TimeSheetDbContext context) => _context = context;

    public Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Attendances.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<Attendance?> GetByEmployeeAndDateAsync(
        Guid employeeId,
        DateOnly date,
        bool track,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Attendances.Where(x => x.EmployeeId == employeeId && x.Date == date && !x.IsDeleted);
        if (!track)
            q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<Attendance>> SearchAsync(
        Guid? employeeId,
        DateOnly? from,
        DateOnly? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Attendances.AsNoTracking().Where(x => !x.IsDeleted);

        if (employeeId is { } eid)
            query = query.Where(x => x.EmployeeId == eid);
        if (from is { } f)
            query = query.Where(x => x.Date >= f);
        if (to is { } t)
            query = query.Where(x => x.Date <= t);

        query = query.OrderByDescending(x => x.Date).ThenBy(x => x.EmployeeId);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<Attendance>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<Attendance>> ListByDateAsync(DateOnly date, CancellationToken cancellationToken = default) =>
        await _context.Attendances.AsNoTracking()
            .Where(x => x.Date == date && !x.IsDeleted)
            .OrderBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Attendance>> ListByEmployeeAsync(
        Guid employeeId,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Attendances.AsNoTracking().Where(x => x.EmployeeId == employeeId && !x.IsDeleted);
        if (from is { } f)
            q = q.Where(x => x.Date >= f);
        if (to is { } t)
            q = q.Where(x => x.Date <= t);
        return await q.OrderByDescending(x => x.Date).ToListAsync(cancellationToken);
    }

    public void Add(Attendance attendance) => _context.Attendances.Add(attendance);
    public void Update(Attendance attendance) => _context.Attendances.Update(attendance);
}
