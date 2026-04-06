using LeaveService.Application.Configuration;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Infrastructure.Persistence;
using LeaveService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LeaveDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<EmployeeServiceOptions>(configuration.GetSection(EmployeeServiceOptions.SectionName));
        services.Configure<AuthServiceOptions>(configuration.GetSection(AuthServiceOptions.SectionName));

        services.AddHttpClient<IEmployeeServiceClient, EmployeeServiceClient>((_, client) =>
        {
            var baseUrl = configuration[$"{EmployeeServiceOptions.SectionName}:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        services.AddHttpClient<IAuthIntegrationClient, AuthIntegrationClient>((_, client) =>
        {
            var baseUrl = configuration[$"{AuthServiceOptions.SectionName}:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        return services;
    }
}
