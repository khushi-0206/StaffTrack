namespace EmployeeService.Application.DTOs.LeaveTypes;

public record CreateLeaveTypeRequestDto(string Name, int MaxDays);

public record UpdateLeaveTypeRequestDto(string Name, int MaxDays);

public record LeaveTypeResponseDto(int Id, string Name, int MaxDays, DateTime CreatedAt, DateTime? UpdatedAt);
