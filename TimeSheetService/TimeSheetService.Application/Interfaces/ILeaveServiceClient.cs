namespace TimeSheetService.Application.Interfaces;

/// <summary>Integrates with Leave Service to detect approved leave days for attendance.</summary>
public interface ILeaveServiceClient
{
    Task<bool> HasApprovedLeaveOnDateAsync(Guid employeeId, DateOnly date, CancellationToken cancellationToken = default);
}
