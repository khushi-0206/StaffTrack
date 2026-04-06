using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;
using AttendanceEntity = TimeSheetService.Domain.Entities.Attendance;

namespace TimeSheetService.Application.Features.Attendance.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ILeaveServiceClient _leaves;
    private readonly ICurrentUserService _current;

    public CheckInCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ILeaveServiceClient leaves,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _leaves = leaves;
        _current = current;
    }

    public async Task<Guid> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var emp = await _employees.GetEmployeeAsync(request.Dto.EmployeeId, cancellationToken);
        if (emp is null)
            throw new NotFoundException("Employee was not found in Employee Service.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanCreateFor(_current, request.Dto.EmployeeId, callerId);

        var date = request.Dto.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        if (await _leaves.HasApprovedLeaveOnDateAsync(request.Dto.EmployeeId, date, cancellationToken))
            throw new ConflictException("This date is covered by approved leave; regular check-in is not expected.");

        var existing = await _uow.Attendances.GetByEmployeeAndDateAsync(request.Dto.EmployeeId, date, track: true, cancellationToken);
        var utcNow = DateTime.UtcNow;

        if (existing is null)
        {
            var row = new AttendanceEntity
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.Dto.EmployeeId,
                Date = date,
                CheckInTime = utcNow,
                CheckOutTime = null,
                TotalHours = null,
                Status = AttendanceStatus.Present
            };
            _uow.Attendances.Add(row);
            await _uow.SaveChangesAsync(cancellationToken);
            return row.Id;
        }

        if (existing.CheckInTime is not null && existing.CheckOutTime is null)
            throw new ConflictException("Already checked in for this date.");

        if (existing.CheckOutTime is not null)
            throw new ConflictException("Attendance for this date is already completed.");

        existing.CheckInTime = utcNow;
        existing.Status = AttendanceStatus.Present;
        _uow.Attendances.Update(existing);
        await _uow.SaveChangesAsync(cancellationToken);
        return existing.Id;
    }
}
