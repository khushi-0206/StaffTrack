using TimeSheetService.Application.Common.Models;
using TimeSheetService.Application.DTOs.Attendance;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceReport;

public record GetAttendanceReportQuery(
    DateOnly From,
    DateOnly To,
    Guid? EmployeeId,
    int Page = 1,
    int PageSize = 50) : IRequest<PagedResult<AttendanceResponseDto>>;
