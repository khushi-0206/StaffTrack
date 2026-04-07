using MediatR;
using Microsoft.Extensions.Logging;

namespace TimeSheetService.Application.Features.Timesheets.Events;

public record TimesheetSubmittedNotification(Guid TimesheetId) : INotification;

public record TimesheetApprovedNotification(Guid TimesheetId) : INotification;

public class TimesheetSubmittedNotificationHandler : INotificationHandler<TimesheetSubmittedNotification>
{
    private readonly ILogger<TimesheetSubmittedNotificationHandler> _logger;

    public TimesheetSubmittedNotificationHandler(ILogger<TimesheetSubmittedNotificationHandler> logger) => _logger = logger;

    public Task Handle(TimesheetSubmittedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timesheet {TimesheetId} submitted (hook for messaging bus).", notification.TimesheetId);
        return Task.CompletedTask;
    }
}

public class TimesheetApprovedNotificationHandler : INotificationHandler<TimesheetApprovedNotification>
{
    private readonly ILogger<TimesheetApprovedNotificationHandler> _logger;

    public TimesheetApprovedNotificationHandler(ILogger<TimesheetApprovedNotificationHandler> logger) => _logger = logger;

    public Task Handle(TimesheetApprovedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timesheet {TimesheetId} approved (hook for messaging bus).", notification.TimesheetId);
        return Task.CompletedTask;
    }
}
