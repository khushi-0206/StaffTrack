using LeaveService.Domain.Enums;

namespace LeaveService.Application.DTOs.LeaveRequests;

public record ApplyLeaveRequestDto(
    Guid EmployeeId,
    int LeaveTypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);

public record LeaveRequestResponseDto(
    Guid Id,
    Guid EmployeeId,
    int LeaveTypeId,
    string LeaveTypeName,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason,
    LeaveRequestStatus Status,
    Guid? ApprovedByEmployeeId,
    DateTime AppliedAt,
    DateTime? UpdatedAt);

public record ApproveLeaveRequestDto(string? Remarks);

public record RejectLeaveRequestDto(string? Remarks);

public record CancelLeaveRequestDto(string? Remarks);
