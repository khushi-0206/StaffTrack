using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveService.Application.Features.LeaveRequests.Events;

public record LeaveAppliedNotification(Guid LeaveRequestId) : INotification;

public record LeaveApprovedNotification(Guid LeaveRequestId) : INotification;

public class LeaveAppliedNotificationHandler : INotificationHandler<LeaveAppliedNotification>
{
    private readonly ILogger<LeaveAppliedNotificationHandler> _logger;

    public LeaveAppliedNotificationHandler(ILogger<LeaveAppliedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(LeaveAppliedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Leave applied: {Id}", notification.LeaveRequestId);
        return Task.CompletedTask;
    }
}

public class LeaveApprovedNotificationHandler : INotificationHandler<LeaveApprovedNotification>
{
    private readonly ILogger<LeaveApprovedNotificationHandler> _logger;

    public LeaveApprovedNotificationHandler(ILogger<LeaveApprovedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(LeaveApprovedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Leave approved: {Id}", notification.LeaveRequestId);
        return Task.CompletedTask;
    }
}
