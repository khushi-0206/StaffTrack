using MediatR;
using NotificationService.Domain.Entities;


namespace NotificationService.Application.Features.Logs.Queries.GetLogsByNotification
{
    public record GetLogsByNotificationQuery(Guid NotificationId) : IRequest<List<NotificationLog>>;
}
