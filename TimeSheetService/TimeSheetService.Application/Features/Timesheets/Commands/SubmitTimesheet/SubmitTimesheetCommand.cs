using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.SubmitTimesheet;

public record SubmitTimesheetCommand(Guid Id) : IRequest<Unit>;
