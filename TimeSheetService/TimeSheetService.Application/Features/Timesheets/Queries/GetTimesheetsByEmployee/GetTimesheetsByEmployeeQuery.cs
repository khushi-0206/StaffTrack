using TimeSheetService.Application.DTOs.Timesheets;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetsByEmployee;

public record GetTimesheetsByEmployeeQuery(Guid EmployeeId) : IRequest<IReadOnlyList<TimesheetResponseDto>>;
