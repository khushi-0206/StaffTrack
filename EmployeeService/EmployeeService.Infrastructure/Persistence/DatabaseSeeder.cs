using EmployeeService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Infrastructure.Persistence;

/// <summary>
/// Seeds reference data after migrations (avoids SQL Server identity conflicts with EF HasData).
/// </summary>
public static class DatabaseSeeder
{
    private static readonly Role[] DefaultRoles =
    {
        new() { Name = "Admin" },
        new() { Name = "HR" },
        new() { Name = "Manager" },
        new() { Name = "Employee" }
    };

    public static async Task SeedAsync(EmployeeDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Roles.AnyAsync(cancellationToken))
            return;

        db.Roles.AddRange(DefaultRoles);
        await db.SaveChangesAsync(cancellationToken);
    }
}
