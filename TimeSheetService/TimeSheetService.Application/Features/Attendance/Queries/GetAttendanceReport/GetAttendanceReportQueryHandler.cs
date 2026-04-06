using AutoMapper;
using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Common.Models;
using TimeSheetService.Application.DTOs.Attendance;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceReport;

public class GetAttendanceReportQueryHandler
    : IRequestHandler<GetAttendanceReportQuery, PagedResult<AttendanceResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetAttendanceReportQueryHandler(
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

    public async Task<PagedResult<AttendanceResponseDto>> Handle(
        GetAttendanceReportQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        if (TimesheetRoles.IsHrOrAdmin(_current.Roles))
        {
            var result = await _uow.Attendances.SearchAsync(
                request.EmployeeId, request.From, request.To, page, pageSize, cancellationToken);
            var items = _mapper.Map<IReadOnlyList<AttendanceResponseDto>>(result.Items.Where(x => !x.IsDeleted).ToList());
            return new PagedResult<AttendanceResponseDto>(items, page, pageSize, result.TotalCount);
        }

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        Guid? filterEmployee = request.EmployeeId;

        if (TimesheetRoles.IsManager(_current.Roles))
        {
            var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
            if (filterEmployee is { } eid && !team.Contains(eid) && eid != callerId)
                throw new ForbiddenAppException();
            if (filterEmployee is null)
            {
                var result = await _uow.Attendances.SearchAsync(null, request.From, request.To, page, 5000, cancellationToken);
                var filtered = result.Items.Where(x => !x.IsDeleted && (team.Contains(x.EmployeeId) || x.EmployeeId == callerId)).ToList();
                var skip = (page - 1) * pageSize;
                var slice = filtered.Skip(skip).Take(pageSize).ToList();
                var dtos = _mapper.Map<IReadOnlyList<AttendanceResponseDto>>(slice);
                return new PagedResult<AttendanceResponseDto>(dtos, page, pageSize, filtered.Count);
            }
        }
        else
        {
            filterEmployee = callerId;
        }

        var paged = await _uow.Attendances.SearchAsync(filterEmployee, request.From, request.To, page, pageSize, cancellationToken);
        var list = _mapper.Map<IReadOnlyList<AttendanceResponseDto>>(paged.Items.Where(x => !x.IsDeleted).ToList());
        return new PagedResult<AttendanceResponseDto>(list, page, pageSize, paged.TotalCount);
    }
}
