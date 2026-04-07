using TimeSheetService.Application.Interfaces;

namespace TimeSheetService.Tests.Support;

public sealed class StubLeaveServiceClient : ILeaveServiceClient
{
    public Task<bool> HasApprovedLeaveOnDateAsync(
        Guid employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default) => Task.FromResult(false);
}
