using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.DTOs.Reports;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.Reports.Queries.GetYearlyLeaveReport;

public class GetYearlyLeaveReportQueryHandler
    : IRequestHandler<GetYearlyLeaveReportQuery, YearlyLeaveReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;

    public GetYearlyLeaveReportQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _current = current;
    }

    public async Task<YearlyLeaveReportDto> Handle(
        GetYearlyLeaveReportQuery request,
        CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Yearly reports require HR or administrator access.");

        var yearStart = new DateOnly(request.Year, 1, 1);
        var yearEnd = new DateOnly(request.Year, 12, 31);

        var page = await _unitOfWork.LeaveRequests.SearchAsync(
            1,
            50_000,
            null,
            null,
            null,
            yearStart,
            yearEnd,
            null,
            false,
            cancellationToken);

        var items = page.Items.Where(x => !x.IsDeleted && x.StartDate <= yearEnd && x.EndDate >= yearStart).ToList();

        return new YearlyLeaveReportDto(
            request.Year,
            items.Count,
            items.Count(x => x.Status == LeaveRequestStatus.Approved),
            items.Count(x => x.Status == LeaveRequestStatus.Pending));
    }
}
