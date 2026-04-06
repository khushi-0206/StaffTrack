using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.UpdateTimeEntry;

public class UpdateTimeEntryCommandHandler : IRequestHandler<UpdateTimeEntryCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public UpdateTimeEntryCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(UpdateTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _uow.TimeEntries.GetByIdWithTimesheetAsync(request.Id, cancellationToken);
        if (entry is null)
            throw new NotFoundException("Time entry not found.");

        var sheet = entry.Timesheet;
        if (sheet.IsDeleted || sheet.Status != TimesheetStatus.Pending)
            throw new AppException("This time entry cannot be modified.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanModifyEntry(_current, sheet, callerId);

        if (request.Dto.ProjectId is { } pid)
        {
            if (await _uow.Projects.GetByIdAsync(pid, cancellationToken) is null)
                throw new NotFoundException("Project not found.");
        }

        var sumOthers = await _uow.TimeEntries.SumHoursForEmployeeOnWorkDateAsync(
            sheet.EmployeeId, request.Dto.WorkDate, request.Id, cancellationToken);

        if (sumOthers + request.Dto.HoursWorked > TimesheetPolicy.MaxHoursPerCalendarDay)
            throw new AppException($"Total hours for {request.Dto.WorkDate} cannot exceed {TimesheetPolicy.MaxHoursPerCalendarDay}.");

        entry.ProjectId = request.Dto.ProjectId;
        entry.ProjectName = request.Dto.ProjectName.Trim();
        entry.TaskDescription = request.Dto.TaskDescription.Trim();
        entry.HoursWorked = request.Dto.HoursWorked;
        entry.WorkDate = request.Dto.WorkDate;
        _uow.TimeEntries.Update(entry);

        await _uow.SaveChangesAsync(cancellationToken);
        await TimesheetTotalsSync.RecalculateAsync(_uow, sheet.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
