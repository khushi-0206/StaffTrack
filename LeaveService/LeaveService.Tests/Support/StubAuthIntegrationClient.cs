using LeaveService.Application.Interfaces;

namespace LeaveService.Tests.Support;

public sealed class StubAuthIntegrationClient : IAuthIntegrationClient
{
    public Task<bool> ValidateCallerAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);
}
