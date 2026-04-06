using TimeSheetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(TimeSheetDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Projects.AnyAsync(cancellationToken))
            return;

        db.Projects.AddRange(
            new Project { Name = "Internal", Description = "Non-billable / admin" },
            new Project { Name = "Client Delivery", Description = "Billable client work" });

        await db.SaveChangesAsync(cancellationToken);
    }
}
