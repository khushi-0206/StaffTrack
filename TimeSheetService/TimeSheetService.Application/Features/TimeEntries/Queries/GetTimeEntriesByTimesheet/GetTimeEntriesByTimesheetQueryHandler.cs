using AutoMapper;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.DTOs.TimeEntries;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Queries.GetTimeEntriesByTimesheet;

public class GetTimeEntriesByTimesheetQueryHandler
    : IRequestHandler<GetTimeEntriesByTimesheetQuery, IReadOnlyList<TimeEntryResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetTimeEntriesByTimesheetQueryHandler(
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

    public async Task<IReadOnlyList<TimeEntryResponseDto>> Handle(
        GetTimeEntriesByTimesheetQuery request,
        CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdAsync(request.TimesheetId, track: false, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            throw new NotFoundException("Timesheet not found.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
        TimesheetAuthorization.EnsureCanViewEmployeeTimesheets(_current, sheet.EmployeeId, callerId, team);

        var list = await _uow.TimeEntries.ListByTimesheetAsync(request.TimesheetId, cancellationToken);
        return _mapper.Map<IReadOnlyList<TimeEntryResponseDto>>(list);
    }
}
