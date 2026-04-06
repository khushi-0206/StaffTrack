using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Commands.ApproveTimesheet;

public record ApproveTimesheetCommand(Guid Id, ApproveTimesheetRequestDto Dto) : IRequest<Unit>;
