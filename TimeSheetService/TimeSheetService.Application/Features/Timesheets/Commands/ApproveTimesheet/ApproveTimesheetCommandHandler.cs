using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Features.Timesheets.Events;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.ApproveTimesheet;

public class ApproveTimesheetCommandHandler : IRequestHandler<ApproveTimesheetCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IPublisher _publisher;

    public ApproveTimesheetCommandHandler(
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

    public async Task<Unit> Handle(ApproveTimesheetCommand request, CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdAsync(request.Id, track: true, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            throw new NotFoundException("Timesheet not found.");

        if (sheet.Status != TimesheetStatus.Submitted)
            throw new AppException("Only submitted timesheets can be approved.");

        var subject = await _employees.GetEmployeeAsync(sheet.EmployeeId, cancellationToken);
        if (subject is null)
            throw new NotFoundException("Employee record not found.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        await TimesheetAuthorization.EnsureCanApproveOrRejectAsync(_current, sheet, subject, callerId, cancellationToken);

        sheet.Status = TimesheetStatus.Approved;
        sheet.ApprovedByEmployeeId = callerId;
        sheet.ApprovedAt = DateTime.UtcNow;
        _uow.Timesheets.Update(sheet);

        _uow.TimesheetHistories.Add(new TimesheetHistory
        {
            TimesheetId = sheet.Id,
            Action = TimesheetHistoryAction.Approved,
            ActionBy = callerId,
            ActionDate = DateTime.UtcNow,
            Remarks = request.Dto.Remarks?.Trim()
        });

        await _uow.SaveChangesAsync(cancellationToken);
        await _publisher.Publish(new TimesheetApprovedNotification(sheet.Id), cancellationToken);
        return Unit.Value;
    }
}
