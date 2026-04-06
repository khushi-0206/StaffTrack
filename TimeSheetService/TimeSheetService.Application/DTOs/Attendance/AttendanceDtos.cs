using TimeSheetService.Domain.Enums;

namespace TimeSheetService.Application.DTOs.Attendance;

public record CheckInRequestDto(Guid EmployeeId, DateOnly? Date);

public record CheckOutRequestDto(Guid EmployeeId, DateOnly? Date);

public record AttendanceResponseDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly Date,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    decimal? TotalHours,
    AttendanceStatus Status);
