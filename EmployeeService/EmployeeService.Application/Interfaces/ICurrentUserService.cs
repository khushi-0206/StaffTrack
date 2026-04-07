namespace EmployeeService.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    /// <summary>Email from JWT (for matching StaffTrack Employee records).</summary>
    string? Email { get; }

    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
}
