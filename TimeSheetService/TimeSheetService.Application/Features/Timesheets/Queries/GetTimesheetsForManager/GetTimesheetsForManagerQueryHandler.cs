using AutoMapper;
using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetsForManager;

public class GetTimesheetsForManagerQueryHandler
    : IRequestHandler<GetTimesheetsForManagerQuery, IReadOnlyList<TimesheetResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetTimesheetsForManagerQueryHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IMapper mapper)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TimesheetResponseDto>> Handle(
        GetTimesheetsForManagerQuery request,
        CancellationToken cancellationToken)
    {
        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles) && callerId != request.ManagerId)
            throw new ForbiddenAppException("You can only load team timesheets for your own manager profile.");

        var ids = await _employees.GetEmployeeIdsForManagerAsync(request.ManagerId, cancellationToken);
        if (ids.Count == 0)
            return Array.Empty<TimesheetResponseDto>();

        var list = await _uow.Timesheets.GetByEmployeeIdsAsync(ids, cancellationToken);
        return _mapper.Map<IReadOnlyList<TimesheetResponseDto>>(list.Where(x => !x.IsDeleted).ToList());
    }
}
