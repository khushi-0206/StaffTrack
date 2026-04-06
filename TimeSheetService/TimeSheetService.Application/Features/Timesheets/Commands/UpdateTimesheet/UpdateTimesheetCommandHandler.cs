using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.UpdateTimesheet;

public class UpdateTimesheetCommandHandler : IRequestHandler<UpdateTimesheetCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public UpdateTimesheetCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(UpdateTimesheetCommand request, CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdAsync(request.Id, track: true, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            throw new NotFoundException("Timesheet not found.");

        if (sheet.Status != TimesheetStatus.Pending)
            throw new AppException("Only pending timesheets can be updated.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanModifyEntry(_current, sheet, callerId);

        if (request.Dto.Date is { } newDate && newDate != sheet.Date)
        {
            if (await _uow.Timesheets.ExistsActiveForEmployeeDateAsync(sheet.EmployeeId, newDate, sheet.Id, cancellationToken))
                throw new ConflictException("Another timesheet already exists for this date.");
            sheet.Date = newDate;
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
