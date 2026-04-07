namespace LeaveService.Application.Interfaces;

public interface IAuthIntegrationClient
{
    Task<bool> ValidateCallerAsync(CancellationToken cancellationToken = default);
}
