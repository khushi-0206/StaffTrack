using TimeSheetService.Application.DTOs.Attendance;
using MediatR;

namespace TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceByEmployee;

public record GetAttendanceByEmployeeQuery(Guid EmployeeId, DateOnly? From, DateOnly? To)
    : IRequest<IReadOnlyList<AttendanceResponseDto>>;
