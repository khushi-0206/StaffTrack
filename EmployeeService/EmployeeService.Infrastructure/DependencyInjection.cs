using EmployeeService.Application.Configuration;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Infrastructure.Persistence;
using EmployeeService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EmployeeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<AuthServiceOptions>(configuration.GetSection(AuthServiceOptions.SectionName));

        services.AddHttpClient<IAuthIntegrationClient, AuthIntegrationClient>((sp, client) =>
        {
            var opt = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthServiceOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(opt.BaseUrl))
                client.BaseAddress = new Uri(opt.BaseUrl.TrimEnd('/') + "/");
        });

        return services;
    }
}
