using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetsForManager;

public record GetTimesheetsForManagerQuery(Guid ManagerId) : IRequest<IReadOnlyList<TimesheetResponseDto>>;
