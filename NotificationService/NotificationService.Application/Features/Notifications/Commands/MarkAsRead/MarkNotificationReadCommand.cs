using MediatR;


namespace NotificationService.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkNotificationReadCommand(Guid Id) : IRequest<bool>;
}
