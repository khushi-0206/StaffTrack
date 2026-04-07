using TimeSheetService.Application.Interfaces;

namespace TimeSheetService.Tests.Support;

public sealed class StubAuthIntegrationClient : IAuthIntegrationClient
{
    public Task<bool> ValidateCallerAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);
}
