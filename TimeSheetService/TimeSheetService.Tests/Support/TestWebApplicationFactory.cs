using TimeSheetService.Application.Interfaces;
using TimeSheetService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace TimeSheetService.Tests.Support;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<TimeSheetDbContext>));
            services.RemoveAll(typeof(TimeSheetDbContext));

            var dbName = "TimeSheetTests_" + Guid.NewGuid();
            services.AddDbContext<TimeSheetDbContext>(o => o.UseInMemoryDatabase(dbName));

            services.RemoveAll(typeof(IEmployeeServiceClient));
            services.AddSingleton<IEmployeeServiceClient, StubEmployeeServiceClient>();

            services.RemoveAll(typeof(ILeaveServiceClient));
            services.AddSingleton<ILeaveServiceClient, StubLeaveServiceClient>();

            services.RemoveAll(typeof(IAuthIntegrationClient));
            services.AddSingleton<IAuthIntegrationClient, StubAuthIntegrationClient>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TimeSheetDbContext>();
        db.Database.EnsureCreated();
        DatabaseSeeder.SeedAsync(db).GetAwaiter().GetResult();
        return host;
    }
}
