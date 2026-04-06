using TimeSheetService.Application.Configuration;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Infrastructure.Persistence;
using TimeSheetService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimeSheetService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TimeSheetDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<EmployeeServiceOptions>(configuration.GetSection(EmployeeServiceOptions.SectionName));
        services.Configure<LeaveServiceOptions>(configuration.GetSection(LeaveServiceOptions.SectionName));
        services.Configure<AuthServiceOptions>(configuration.GetSection(AuthServiceOptions.SectionName));

        services.AddHttpClient<IEmployeeServiceClient, EmployeeServiceClient>((_, client) =>
        {
            var baseUrl = configuration[$"{EmployeeServiceOptions.SectionName}:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        services.AddHttpClient<ILeaveServiceClient, LeaveServiceClient>((_, client) =>
        {
            var baseUrl = configuration[$"{LeaveServiceOptions.SectionName}:BaseUrl"];
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
