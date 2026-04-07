using LeaveService.Application.Common.Models;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using LeaveService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Infrastructure.Persistence.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly LeaveDbContext _context;

    public LeaveRequestRepository(LeaveDbContext context)
    {
        _context = context;
    }

    public Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.LeaveRequests.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public Task<LeaveRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.LeaveRequests
            .Include(x => x.LeaveType)
            .Include(x => x.Histories)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<LeaveRequest>> SearchAsync(
        int page,
        int pageSize,
        Guid? employeeId,
        LeaveRequestStatus? status,
        int? leaveTypeId,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.LeaveType)
            .Where(x => !x.IsDeleted);

        if (employeeId is { } eid)
            query = query.Where(x => x.EmployeeId == eid);

        if (status is { } st)
            query = query.Where(x => x.Status == st);

        if (leaveTypeId is { } lt)
            query = query.Where(x => x.LeaveTypeId == lt);

        if (fromDate is { } from && toDate is { } to)
            query = query.Where(x => x.StartDate <= to && x.EndDate >= from);
        else if (fromDate is { } fromOnly)
            query = query.Where(x => x.EndDate >= fromOnly);
        else if (toDate is { } toOnly)
            query = query.Where(x => x.StartDate <= toOnly);

        query = (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "startdate" => sortDescending
                ? query.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.AppliedAt)
                : query.OrderBy(x => x.StartDate).ThenByDescending(x => x.AppliedAt),
            "enddate" => sortDescending
                ? query.OrderByDescending(x => x.EndDate).ThenByDescending(x => x.AppliedAt)
                : query.OrderBy(x => x.EndDate).ThenByDescending(x => x.AppliedAt),
            "status" => sortDescending
                ? query.OrderByDescending(x => x.Status).ThenByDescending(x => x.AppliedAt)
                : query.OrderBy(x => x.Status).ThenByDescending(x => x.AppliedAt),
            _ => sortDescending
                ? query.OrderByDescending(x => x.AppliedAt)
                : query.OrderBy(x => x.AppliedAt)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<LeaveRequest>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetByEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdsAsync(
        IReadOnlyCollection<Guid> employeeIds,
        CancellationToken cancellationToken = default)
    {
        if (employeeIds.Count == 0)
            return Array.Empty<LeaveRequest>();

        return await _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.LeaveType)
            .Where(x => employeeIds.Contains(x.EmployeeId) && !x.IsDeleted)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasOverlappingAsync(
        Guid employeeId,
        DateOnly start,
        DateOnly end,
        LeaveRequestStatus[] blockingStatuses,
        Guid? excludeRequestId,
        CancellationToken cancellationToken = default)
    {
        var q = _context.LeaveRequests
            .Where(x =>
                x.EmployeeId == employeeId
                && !x.IsDeleted
                && blockingStatuses.Contains(x.Status)
                && x.StartDate <= end
                && x.EndDate >= start);

        if (excludeRequestId is { } ex)
            q = q.Where(x => x.Id != ex);

        return q.AnyAsync(cancellationToken);
    }

    public Task<int> CountByLeaveTypeAsync(int leaveTypeId, CancellationToken cancellationToken = default) =>
        _context.LeaveRequests.CountAsync(x => x.LeaveTypeId == leaveTypeId, cancellationToken);

    public void Add(LeaveRequest request) => _context.LeaveRequests.Add(request);

    public void Update(LeaveRequest request) => _context.LeaveRequests.Update(request);
}
