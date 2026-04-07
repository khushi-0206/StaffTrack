using LeaveService.Application.External;
using LeaveService.Application.Interfaces;

namespace LeaveService.Tests.Support;

/// <summary>Deterministic employee data for integration tests (matches JWT email in <see cref="JwtTokenBuilder"/>).</summary>
public sealed class StubEmployeeServiceClient : IEmployeeServiceClient
{
    public static readonly Guid TestEmployeeId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public Task<EmployeeApiProfile?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        Task.FromResult<EmployeeApiProfile?>(ProfileFor(employeeId));

    public Task<EmployeeApiProfile?> GetSelfEmployeeAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<EmployeeApiProfile?>(ProfileFor(TestEmployeeId));

    public Task<IReadOnlyList<Guid>> GetEmployeeIdsForManagerAsync(
        Guid managerEmployeeId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Guid>>(new[] { TestEmployeeId });

    public Task<IReadOnlyList<Guid>> GetEmployeeIdsForDepartmentAsync(
        int departmentId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Guid>>(new[] { TestEmployeeId });

    private static EmployeeApiProfile ProfileFor(Guid id) => new()
    {
        Id = id,
        Email = "tester@stafftrack.local",
        FirstName = "Test",
        LastName = "User",
        DepartmentId = 1,
        ManagerId = null
    };
}
