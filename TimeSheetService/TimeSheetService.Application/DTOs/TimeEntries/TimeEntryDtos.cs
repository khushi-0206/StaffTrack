namespace TimeSheetService.Application.DTOs.TimeEntries;

public record CreateTimeEntryRequestDto(
    Guid TimesheetId,
    int? ProjectId,
    string ProjectName,
    string TaskDescription,
    decimal HoursWorked,
    DateOnly WorkDate);

public record UpdateTimeEntryRequestDto(
    int? ProjectId,
    string ProjectName,
    string TaskDescription,
    decimal HoursWorked,
    DateOnly WorkDate);

public record TimeEntryResponseDto(
    Guid Id,
    Guid TimesheetId,
    int? ProjectId,
    string ProjectName,
    string TaskDescription,
    decimal HoursWorked,
    DateOnly WorkDate);
