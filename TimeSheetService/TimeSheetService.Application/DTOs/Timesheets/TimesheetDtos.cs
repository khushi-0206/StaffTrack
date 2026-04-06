using TimeSheetService.Domain.Enums;

namespace TimeSheetService.Application.DTOs.Timesheets;

public record CreateTimesheetRequestDto(Guid EmployeeId, DateOnly Date);

public record UpdateTimesheetRequestDto(DateOnly? Date);

public record TimesheetResponseDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly Date,
    decimal TotalHours,
    TimesheetStatus Status,
    DateTime? SubmittedAt,
    Guid? ApprovedByEmployeeId,
    DateTime? ApprovedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record RejectTimesheetRequestDto(string? Remarks);

public record ApproveTimesheetRequestDto(string? Remarks);
