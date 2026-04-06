namespace LeaveService.Application.DTOs.LeaveBalance;

public record LeaveBalanceResponseDto(
    int Id,
    Guid EmployeeId,
    int LeaveTypeId,
    string LeaveTypeName,
    int TotalDays,
    int UsedDays,
    int RemainingDays,
    DateTime UpdatedAt);

public record UpdateLeaveBalanceRequestDto(
    Guid EmployeeId,
    int LeaveTypeId,
    int TotalDays,
    int UsedDays);
