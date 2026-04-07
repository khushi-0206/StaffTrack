using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Helpers;

/// <summary>
/// Creates an in-memory <see cref="AuthDbContext"/> with seeded roles (via model HasData).
/// </summary>
public static class TestAuthDbContextFactory
{
    public static AuthDbContext Create(string? databaseName = null)
    {
        var name = databaseName ?? Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(name)
            .Options;

        var ctx = new AuthDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }
}
