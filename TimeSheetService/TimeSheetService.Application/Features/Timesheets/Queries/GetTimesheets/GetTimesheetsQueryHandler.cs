using AutoMapper;
using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Common.Models;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheets;

public class GetTimesheetsQueryHandler : IRequestHandler<GetTimesheetsQuery, PagedResult<TimesheetResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetTimesheetsQueryHandler(IUnitOfWork uow, ICurrentUserService current, IMapper mapper)
    {
        _uow = uow;
        _current = current;
        _mapper = mapper;
    }

    public async Task<PagedResult<TimesheetResponseDto>> Handle(GetTimesheetsQuery request, CancellationToken cancellationToken)
    {
        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Only HR or administrators can list all timesheets.");

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var result = await _uow.Timesheets.SearchAsync(
            page, pageSize, request.EmployeeId, request.Status, request.FromDate, request.ToDate,
            request.SortBy, request.SortDescending, cancellationToken);

        var items = _mapper.Map<IReadOnlyList<TimesheetResponseDto>>(result.Items);
        return new PagedResult<TimesheetResponseDto>(items, page, pageSize, result.TotalCount);
    }
}
