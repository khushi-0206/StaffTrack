using LeaveService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    /// <summary>Seeds default leave types when the catalog is empty.</summary>
    public static async Task SeedAsync(LeaveDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.LeaveTypes.AnyAsync(cancellationToken))
            return;

        db.LeaveTypes.AddRange(
            new LeaveType { Name = "Sick", MaxDays = 10 },
            new LeaveType { Name = "Casual", MaxDays = 12 },
            new LeaveType { Name = "Earned", MaxDays = 15 });

        await db.SaveChangesAsync(cancellationToken);
    }
}
