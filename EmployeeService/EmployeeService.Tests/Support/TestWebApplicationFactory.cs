using EmployeeService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmployeeService.Tests.Support;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<EmployeeDbContext>));
            services.RemoveAll(typeof(EmployeeDbContext));

            var dbName = "EmployeeTests_" + Guid.NewGuid();
            services.AddDbContext<EmployeeDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });
        });
    }
}
