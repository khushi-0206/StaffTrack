using TimeSheetService.Application.DTOs.Attendance;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceByDate;

public record GetAttendanceByDateQuery(DateOnly Date) : IRequest<IReadOnlyList<AttendanceResponseDto>>;
