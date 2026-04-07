using NotificationService.API.Extensions;
using NotificationService.API.Middleware;
using NotificationService.Application;
using NotificationService.Application.DependencyInjection;
using NotificationService.Application.Features.Templates.Commands.CreateTemplate;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTemplateCommand).Assembly));

// 🔹 JWT Authentication (match AuthService)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001"; // AuthService URL
        options.RequireHttpsMetadata = false;
        options.Audience = "notification-api";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 🔹 Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();