namespace EmployeeService.Application.Interfaces;

/// <summary>
/// Calls StaffTrack Auth Service to re-validate the caller's JWT (optional cross-check).
/// </summary>
public interface IAuthIntegrationClient
{
    Task<bool> ValidateCallerAsync(CancellationToken cancellationToken = default);
}
