using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Commands.CheckOut;

public class CheckOutCommandHandler : IRequestHandler<CheckOutCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public CheckOutCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(CheckOutCommand request, CancellationToken cancellationToken)
    {
        var emp = await _employees.GetEmployeeAsync(request.Dto.EmployeeId, cancellationToken);
        if (emp is null)
            throw new NotFoundException("Employee was not found in Employee Service.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanCreateFor(_current, request.Dto.EmployeeId, callerId);

        var date = request.Dto.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var row = await _uow.Attendances.GetByEmployeeAndDateAsync(request.Dto.EmployeeId, date, track: true, cancellationToken);
        if (row is null || row.CheckInTime is null)
            throw new AppException("No check-in found for this date.");

        if (row.CheckOutTime is not null)
            throw new AppException("Already checked out for this date.");

        var checkout = DateTime.UtcNow;
        if (checkout < row.CheckInTime.Value)
            throw new AppException("Check-out time must be after check-in.");

        var hours = (decimal)(checkout - row.CheckInTime.Value).TotalHours;
        row.CheckOutTime = checkout;
        row.TotalHours = Math.Round(hours, 2, MidpointRounding.AwayFromZero);
        _uow.Attendances.Update(row);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
