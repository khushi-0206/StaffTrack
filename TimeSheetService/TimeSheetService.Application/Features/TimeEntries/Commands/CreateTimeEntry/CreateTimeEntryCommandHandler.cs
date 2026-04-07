using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using TimeSheetService.Domain.Enums;
using MediatR;
using TimeEntryEntity = TimeSheetService.Domain.Entities.TimeEntry;

namespace TimeSheetService.Application.Features.TimeEntries.Commands.CreateTimeEntry;

public class CreateTimeEntryCommandHandler : IRequestHandler<CreateTimeEntryCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public CreateTimeEntryCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
    }

    public async Task<Guid> Handle(CreateTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var sheet = await _uow.Timesheets.GetByIdWithDetailsAsync(request.Dto.TimesheetId, cancellationToken);
        if (sheet is null || sheet.IsDeleted)
            throw new NotFoundException("Timesheet not found.");

        if (sheet.Status != TimesheetStatus.Pending)
            throw new AppException("Time entries can only be added to pending timesheets.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanModifyEntry(_current, sheet, callerId);

        if (request.Dto.ProjectId is { } pid)
        {
            if (await _uow.Projects.GetByIdAsync(pid, cancellationToken) is null)
                throw new NotFoundException("Project not found.");
        }

        var currentDaySum = await _uow.TimeEntries.SumHoursForEmployeeOnWorkDateAsync(
            sheet.EmployeeId, request.Dto.WorkDate, null, cancellationToken);

        if (currentDaySum + request.Dto.HoursWorked > TimesheetPolicy.MaxHoursPerCalendarDay)
            throw new AppException($"Total hours for {request.Dto.WorkDate} cannot exceed {TimesheetPolicy.MaxHoursPerCalendarDay}.");

        var entry = new TimeEntryEntity
        {
            Id = Guid.NewGuid(),
            TimesheetId = sheet.Id,
            ProjectId = request.Dto.ProjectId,
            ProjectName = request.Dto.ProjectName.Trim(),
            TaskDescription = request.Dto.TaskDescription.Trim(),
            HoursWorked = request.Dto.HoursWorked,
            WorkDate = request.Dto.WorkDate
        };

        _uow.TimeEntries.Add(entry);
        await _uow.SaveChangesAsync(cancellationToken);

        await TimesheetTotalsSync.RecalculateAsync(_uow, sheet.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}
