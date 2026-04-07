using TimeSheetService.Application.Common.Models;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using TimeSheetService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetService.Infrastructure.Persistence.Repositories;

public class TimesheetRepository : ITimesheetRepository
{
    private readonly TimeSheetDbContext _context;

    public TimesheetRepository(TimeSheetDbContext context) => _context = context;

    public Task<Timesheet?> GetByIdAsync(Guid id, bool track, CancellationToken cancellationToken = default)
    {
        var q = _context.Timesheets.AsQueryable();
        if (!track)
            q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public Task<Timesheet?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Timesheets
            .Include(x => x.Entries)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<bool> ExistsActiveForEmployeeDateAsync(
        Guid employeeId,
        DateOnly date,
        Guid? excludeTimesheetId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.Timesheets.Where(x => !x.IsDeleted && x.EmployeeId == employeeId && x.Date == date);
        if (excludeTimesheetId is { } ex)
            q = q.Where(x => x.Id != ex);
        return q.AnyAsync(cancellationToken);
    }

    public async Task<PagedResult<Timesheet>> SearchAsync(
        int page,
        int pageSize,
        Guid? employeeId,
        TimesheetStatus? status,
        DateOnly? from,
        DateOnly? to,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Timesheets.AsNoTracking().Where(x => !x.IsDeleted);

        if (employeeId is { } eid)
            query = query.Where(x => x.EmployeeId == eid);
        if (status is { } st)
            query = query.Where(x => x.Status == st);
        if (from is { } f && to is { } t)
            query = query.Where(x => x.Date >= f && x.Date <= t);
        else if (from is { } fOnly)
            query = query.Where(x => x.Date >= fOnly);
        else if (to is { } tOnly)
            query = query.Where(x => x.Date <= tOnly);

        query = (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "date" => sortDescending ? query.OrderByDescending(x => x.Date) : query.OrderBy(x => x.Date),
            "totalhours" => sortDescending ? query.OrderByDescending(x => x.TotalHours) : query.OrderBy(x => x.TotalHours),
            "status" => sortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => sortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<Timesheet>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<Timesheet>> GetByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await _context.Timesheets.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.Date)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Timesheet>> GetByEmployeeIdsAsync(
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken cancellationToken = default)
    {
        if (employeeIds.Count == 0)
            return Array.Empty<Timesheet>();
        return await _context.Timesheets.AsNoTracking()
            .Where(x => employeeIds.Contains(x.EmployeeId) && !x.IsDeleted)
            .OrderByDescending(x => x.Date)
            .ToListAsync(cancellationToken);
    }

    public void Add(Timesheet timesheet) => _context.Timesheets.Add(timesheet);
    public void Update(Timesheet timesheet) => _context.Timesheets.Update(timesheet);
}
