using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.DTOs.Reports;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetMonthlyLeaveReport;

public class GetMonthlyLeaveReportQueryHandler
    : IRequestHandler<GetMonthlyLeaveReportQuery, MonthlyLeaveReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;

    public GetMonthlyLeaveReportQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _current = current;
    }

    public async Task<MonthlyLeaveReportDto> Handle(
        GetMonthlyLeaveReportQuery request,
        CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Monthly reports require HR or administrator access.");

        var monthStart = new DateOnly(request.Year, request.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var page = await _unitOfWork.LeaveRequests.SearchAsync(
            1,
            10_000,
            null,
            null,
            null,
            monthStart,
            monthEnd,
            null,
            false,
            cancellationToken);

        var items = page.Items.Where(x => !x.IsDeleted && x.StartDate <= monthEnd && x.EndDate >= monthStart).ToList();

        return new MonthlyLeaveReportDto(
            request.Year,
            request.Month,
            items.Count,
            items.Count(x => x.Status == LeaveRequestStatus.Approved),
            items.Count(x => x.Status == LeaveRequestStatus.Pending));
    }
}
