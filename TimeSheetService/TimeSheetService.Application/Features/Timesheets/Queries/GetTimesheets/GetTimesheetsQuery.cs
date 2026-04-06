using TimeSheetService.Application.Common.Models;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Domain.Enums;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheets;

public record GetTimesheetsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? EmployeeId = null,
    TimesheetStatus? Status = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string? SortBy = null,
    bool SortDescending = false) : IRequest<PagedResult<TimesheetResponseDto>>;
