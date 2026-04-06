using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.DTOs.Reports;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetDepartmentLeaveReport;

public class GetDepartmentLeaveReportQueryHandler
    : IRequestHandler<GetDepartmentLeaveReportQuery, DepartmentLeaveReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public GetDepartmentLeaveReportQueryHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
    }

    public async Task<DepartmentLeaveReportDto> Handle(
        GetDepartmentLeaveReportQuery request,
        CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Department reports require HR or administrator access.");

        var ids = await _employees.GetEmployeeIdsForDepartmentAsync(request.DepartmentId, cancellationToken);
        if (ids.Count == 0)
            return new DepartmentLeaveReportDto(request.DepartmentId, 0, 0, 0, Array.Empty<LeaveRequestSummaryDto>());

        var leaves = await _unitOfWork.LeaveRequests.GetByEmployeeIdsAsync(ids, cancellationToken);
        var filtered = leaves.Where(l => !l.IsDeleted).ToList();

        if (request.From is { } from && request.To is { } to)
            filtered = filtered.Where(l => l.StartDate <= to && l.EndDate >= from).ToList();

        var approvedDays = 0;
        foreach (var l in filtered.Where(x => x.Status == LeaveRequestStatus.Approved))
        {
            var overlapStart = request.From is { } f ? (l.StartDate > f ? l.StartDate : f) : l.StartDate;
            var overlapEnd = request.To is { } t ? (l.EndDate < t ? l.EndDate : t) : l.EndDate;
            if (overlapStart <= overlapEnd)
                approvedDays += overlapEnd.DayNumber - overlapStart.DayNumber + 1;
        }

        var sample = filtered.OrderByDescending(x => x.AppliedAt).Take(25).ToList();

        return new DepartmentLeaveReportDto(
            request.DepartmentId,
            ids.Count,
            filtered.Count,
            approvedDays,
            sample.Select(x => new LeaveRequestSummaryDto(
                x.Id,
                x.LeaveTypeId,
                x.LeaveType?.Name ?? "",
                x.StartDate,
                x.EndDate,
                x.Status,
                x.AppliedAt)).ToList());
    }
}
