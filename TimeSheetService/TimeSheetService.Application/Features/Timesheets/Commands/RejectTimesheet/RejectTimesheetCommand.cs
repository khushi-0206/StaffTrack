using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.RejectTimesheet;

public record RejectTimesheetCommand(Guid Id, RejectTimesheetRequestDto Dto) : IRequest<Unit>;
