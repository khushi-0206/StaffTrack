using AutoMapper;
using TimeSheetService.Application.DTOs.Attendance;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceByEmployee;

public class GetAttendanceByEmployeeQueryHandler
    : IRequestHandler<GetAttendanceByEmployeeQuery, IReadOnlyList<AttendanceResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetAttendanceByEmployeeQueryHandler(
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

    public async Task<IReadOnlyList<AttendanceResponseDto>> Handle(
        GetAttendanceByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
        TimesheetAuthorization.EnsureCanViewEmployeeTimesheets(_current, request.EmployeeId, callerId, team);

        var list = await _uow.Attendances.ListByEmployeeAsync(request.EmployeeId, request.From, request.To, cancellationToken);
        return _mapper.Map<IReadOnlyList<AttendanceResponseDto>>(list.Where(x => !x.IsDeleted).ToList());
    }
}
