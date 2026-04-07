using MediatR;
using NotificationService.Application.Common.DTOs;

namespace NotificationService.Application.Features.Notifications.Queries.GetUserNotification
{
    public record GetUserNotificationsQuery(Guid UserId) : IRequest<List<NotificationDto>>;
}
