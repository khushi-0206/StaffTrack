using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.CreateTimesheet;

public record CreateTimesheetCommand(CreateTimesheetRequestDto Dto) : IRequest<Guid>;
