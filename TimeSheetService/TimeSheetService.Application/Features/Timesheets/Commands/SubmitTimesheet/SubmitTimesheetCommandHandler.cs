using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Features.Timesheets.Events;
using TimeSheetService.Domain.Entities;
using TimeSheetService.Domain.Enums;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.SubmitTimesheet;

public class SubmitTimesheetCommandHandler : IRequestHandler<SubmitTimesheetCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IPublisher _publisher;

    public SubmitTimesheetCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IPublisher publisher)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(SubmitTimesheetCommand request, CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            throw new NotFoundException("Timesheet not found.");

        if (sheet.Status != TimesheetStatus.Pending)
            throw new AppException("Only pending timesheets can be submitted.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanModifyEntry(_current, sheet, callerId);

        await TimesheetTotalsSync.RecalculateAsync(_uow, sheet.Id, cancellationToken);

        sheet = await _uow.Timesheets.GetByIdAsync(sheet.Id, track: true, cancellationToken)
                ?? throw new NotFoundException("Timesheet not found.");

        if (sheet.TotalHours <= 0)
            throw new AppException("Add time entries before submitting.");

        sheet.Status = TimesheetStatus.Submitted;
        sheet.SubmittedAt = DateTime.UtcNow;
        _uow.Timesheets.Update(sheet);

        _uow.TimesheetHistories.Add(new TimesheetHistory
        {
            TimesheetId = sheet.Id,
            Action = TimesheetHistoryAction.Submitted,
            ActionBy = callerId,
            ActionDate = DateTime.UtcNow,
            Remarks = null
        });

        await _uow.SaveChangesAsync(cancellationToken);
        await _publisher.Publish(new TimesheetSubmittedNotification(sheet.Id), cancellationToken);
        return Unit.Value;
    }
}
