using LeaveService.Application.Interfaces;
using LeaveService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LeaveService.Tests.Support;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<LeaveDbContext>));
            services.RemoveAll(typeof(LeaveDbContext));

            var dbName = "LeaveTests_" + Guid.NewGuid();
            services.AddDbContext<LeaveDbContext>(options => options.UseInMemoryDatabase(dbName));

            services.RemoveAll(typeof(IEmployeeServiceClient));
            services.AddSingleton<IEmployeeServiceClient, StubEmployeeServiceClient>();

            services.RemoveAll(typeof(IAuthIntegrationClient));
            services.AddSingleton<IAuthIntegrationClient, StubAuthIntegrationClient>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LeaveDbContext>();
        db.Database.EnsureCreated();
        DatabaseSeeder.SeedAsync(db).GetAwaiter().GetResult();
        return host;
    }
}
