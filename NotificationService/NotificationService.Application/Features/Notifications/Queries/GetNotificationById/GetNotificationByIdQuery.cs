using MediatR;
using NotificationService.Application.Common.DTOs;


namespace NotificationService.Application.Features.Notifications.Queries.GetNotificationById
{
    public record GetNotificationByIdQuery(Guid Id) : IRequest<NotificationDto>;
}
