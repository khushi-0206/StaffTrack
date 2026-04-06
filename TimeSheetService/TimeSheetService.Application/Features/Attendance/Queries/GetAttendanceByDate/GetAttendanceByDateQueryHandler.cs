using AutoMapper;
using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.DTOs.Attendance;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceByDate;

public class GetAttendanceByDateQueryHandler
    : IRequestHandler<GetAttendanceByDateQuery, IReadOnlyList<AttendanceResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetAttendanceByDateQueryHandler(IUnitOfWork uow, ICurrentUserService current, IMapper mapper)
    {
        _uow = uow;
        _current = current;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AttendanceResponseDto>> Handle(
        GetAttendanceByDateQuery request,
        CancellationToken cancellationToken)
    {
        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Only HR or administrators can list attendance for all employees on a date.");

        var list = await _uow.Attendances.ListByDateAsync(request.Date, cancellationToken);
        return _mapper.Map<IReadOnlyList<AttendanceResponseDto>>(list.Where(x => !x.IsDeleted).ToList());
    }
}
