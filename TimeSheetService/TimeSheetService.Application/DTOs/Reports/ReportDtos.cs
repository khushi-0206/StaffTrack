namespace TimeSheetService.Application.DTOs.Reports;

public record EmployeeTimesheetReportDto(
    Guid EmployeeId,
    int TimesheetCount,
    decimal TotalHoursSubmitted,
    decimal TotalHoursApproved,
    IReadOnlyList<TimesheetSummaryDto> RecentSheets);

public record TimesheetSummaryDto(Guid Id, DateOnly Date, decimal TotalHours, string StatusLabel);

public record WeeklyReportDto(int Year, int Week, IReadOnlyList<EmployeeHoursDto> ByEmployee);

public record MonthlyReportDto(int Year, int Month, IReadOnlyList<EmployeeHoursDto> ByEmployee);

public record EmployeeHoursDto(Guid EmployeeId, decimal TotalHours, int TimesheetCount);

public record ProductivityReportDto(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<EmployeeHoursDto> ApprovedHoursByEmployee);
