using TimeSheetService.Application.Common;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.DeleteTimesheet;

public class DeleteTimesheetCommandHandler : IRequestHandler<DeleteTimesheetCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public DeleteTimesheetCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(DeleteTimesheetCommand request, CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdAsync(request.Id, track: true, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            throw new NotFoundException("Timesheet not found.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);

        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
        {
            if (sheet.EmployeeId != callerId || sheet.Status != TimesheetStatus.Pending)
                throw new ForbiddenAppException("You cannot delete this timesheet.");
        }

        sheet.IsDeleted = true;
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
