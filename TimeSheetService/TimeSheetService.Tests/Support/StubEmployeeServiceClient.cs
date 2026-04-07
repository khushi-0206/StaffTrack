using TimeSheetService.Application.External;
using TimeSheetService.Application.Interfaces;

namespace TimeSheetService.Tests.Support;

public sealed class StubEmployeeServiceClient : IEmployeeServiceClient
{
    public static readonly Guid TestEmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public Task<EmployeeApiProfile?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        Task.FromResult<EmployeeApiProfile?>(Profile(employeeId));

    public Task<EmployeeApiProfile?> GetSelfEmployeeAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<EmployeeApiProfile?>(Profile(TestEmployeeId));

    public Task<IReadOnlyList<Guid>> GetEmployeeIdsForManagerAsync(
        Guid managerEmployeeId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Guid>>(new[] { TestEmployeeId });

    public Task<IReadOnlyList<Guid>> GetEmployeeIdsForDepartmentAsync(
        int departmentId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Guid>>(new[] { TestEmployeeId });

    private static EmployeeApiProfile Profile(Guid id) => new()
    {
        Id = id,
        Email = "tester@stafftrack.local",
        FirstName = "Test",
        LastName = "User",
        DepartmentId = 1,
        ManagerId = null
    };
}
