using LeaveService.Domain.Enums;

namespace LeaveService.Application.DTOs.Reports;

public record EmployeeLeaveReportDto(
    Guid EmployeeId,
    int TotalRequests,
    int PendingCount,
    int ApprovedCount,
    int RejectedCount,
    int CancelledCount,
    IReadOnlyList<LeaveRequestSummaryDto> RecentRequests);

public record LeaveRequestSummaryDto(
    Guid Id,
    int LeaveTypeId,
    string LeaveTypeName,
    DateOnly StartDate,
    DateOnly EndDate,
    LeaveRequestStatus Status,
    DateTime AppliedAt);

public record DepartmentLeaveReportDto(
    int DepartmentId,
    int EmployeeCount,
    int TotalLeaveRequests,
    int ApprovedDaysInPeriod,
    IReadOnlyList<LeaveRequestSummaryDto> SampleRequests);

public record MonthlyLeaveReportDto(int Year, int Month, int TotalRequests, int ApprovedRequests, int PendingRequests);

public record YearlyLeaveReportDto(int Year, int TotalRequests, int ApprovedRequests, int PendingRequests);
