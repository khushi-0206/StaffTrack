using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeService.Application.Features.Employees.Events;

public class EmployeeCreatedNotificationHandler : INotificationHandler<EmployeeCreatedNotification>
{
    private readonly ILogger<EmployeeCreatedNotificationHandler> _logger;

    public EmployeeCreatedNotificationHandler(ILogger<EmployeeCreatedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EmployeeCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Employee created: {EmployeeId}", notification.EmployeeId);
        return Task.CompletedTask;
    }
}

public class EmployeeUpdatedNotificationHandler : INotificationHandler<EmployeeUpdatedNotification>
{
    private readonly ILogger<EmployeeUpdatedNotificationHandler> _logger;

    public EmployeeUpdatedNotificationHandler(ILogger<EmployeeUpdatedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EmployeeUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Employee updated: {EmployeeId}", notification.EmployeeId);
        return Task.CompletedTask;
    }
}
