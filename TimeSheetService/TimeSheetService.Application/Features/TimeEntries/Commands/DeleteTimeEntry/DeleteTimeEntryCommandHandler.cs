using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.DeleteTimeEntry;

public class DeleteTimeEntryCommandHandler : IRequestHandler<DeleteTimeEntryCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public DeleteTimeEntryCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(DeleteTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _uow.TimeEntries.GetByIdWithTimesheetAsync(request.Id, cancellationToken);
        if (entry is null)
            throw new NotFoundException("Time entry not found.");

        var sheet = entry.Timesheet;
        if (sheet.IsDeleted || sheet.Status != TimesheetStatus.Pending)
            throw new AppException("This time entry cannot be deleted.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanModifyEntry(_current, sheet, callerId);

        _uow.TimeEntries.Remove(entry);
        await _uow.SaveChangesAsync(cancellationToken);
        await TimesheetTotalsSync.RecalculateAsync(_uow, sheet.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
