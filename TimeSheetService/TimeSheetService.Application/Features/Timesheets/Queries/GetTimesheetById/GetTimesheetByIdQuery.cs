using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetById;

public record GetTimesheetByIdQuery(Guid Id) : IRequest<TimesheetResponseDto?>;
