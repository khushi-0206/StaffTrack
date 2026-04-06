using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.UpdateTimesheet;

public record UpdateTimesheetCommand(Guid Id, UpdateTimesheetRequestDto Dto) : IRequest<Unit>;
