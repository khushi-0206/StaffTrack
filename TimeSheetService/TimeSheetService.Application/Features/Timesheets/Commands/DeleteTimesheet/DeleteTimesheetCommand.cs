using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.DeleteTimesheet;

public record DeleteTimesheetCommand(Guid Id) : IRequest<Unit>;
