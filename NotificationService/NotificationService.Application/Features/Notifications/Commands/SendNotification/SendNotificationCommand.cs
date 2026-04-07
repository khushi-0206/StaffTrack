using NotificationService.Domain.Enums;
using MediatR;

namespace NotificationService.Application.Features.Notifications.Commands.SendNotification
{
    public record SendNotificationCommand(
    string Title,
    string Message,
    NotificationType Type,
    Guid RecipientId
) : IRequest<Guid>;
}
