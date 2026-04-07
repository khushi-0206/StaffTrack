using AutoMapper;
using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetById;

public class GetTimesheetByIdQueryHandler : IRequestHandler<GetTimesheetByIdQuery, TimesheetResponseDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetTimesheetByIdQueryHandler(
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

    public async Task<TimesheetResponseDto?> Handle(GetTimesheetByIdQuery request, CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdAsync(request.Id, track: false, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            return null;

        if (TimesheetRoles.IsHrOrAdmin(_current.Roles))
            return _mapper.Map<TimesheetResponseDto>(sheet);

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        if (sheet.EmployeeId == callerId)
            return _mapper.Map<TimesheetResponseDto>(sheet);

        if (TimesheetRoles.IsManager(_current.Roles))
        {
            var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
            if (team.Contains(sheet.EmployeeId))
                return _mapper.Map<TimesheetResponseDto>(sheet);
        }

        throw new ForbiddenAppException();
    }
}
