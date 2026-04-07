using TimeSheetService.Application.DTOs.TimeEntries;
using MediatR;

namespace TimeSheetService.Application.Features.TimeEntries.Queries.GetTimeEntriesByTimesheet;

public record GetTimeEntriesByTimesheetQuery(Guid TimesheetId) : IRequest<IReadOnlyList<TimeEntryResponseDto>>;
