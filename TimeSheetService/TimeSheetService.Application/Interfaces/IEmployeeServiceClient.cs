using TimeSheetService.Application.External;

namespace TimeSheetService.Application.Interfaces;

public interface IEmployeeServiceClient
{
    Task<EmployeeApiProfile?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<EmployeeApiProfile?> GetSelfEmployeeAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetEmployeeIdsForManagerAsync(Guid managerEmployeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetEmployeeIdsForDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
}
