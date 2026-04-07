using EmployeeService.Domain.Entities;
using EmployeeService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Tests.Support;

/// <summary>Shared in-memory DB setup for API integration tests.</summary>
internal static class ApiIntegrationDb
{
    /// <summary>Seeds default roles and one Engineering department; returns department id and Employee role id.</summary>
    public static async Task<(int DepartmentId, int EmployeeRoleId)> SeedDepartmentAndRolesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        await DatabaseSeeder.SeedAsync(db, cancellationToken);

        if (!await db.Departments.AnyAsync(d => d.Name == "Engineering", cancellationToken))
        {
            db.Departments.Add(new Department { Name = "Engineering", Description = "R&D" });
            await db.SaveChangesAsync(cancellationToken);
        }

        var dept = await db.Departments.FirstAsync(d => d.Name == "Engineering", cancellationToken);
        var role = await db.Roles.FirstAsync(r => r.Name == "Employee", cancellationToken);
        return (dept.Id, role.Id);
    }
}
