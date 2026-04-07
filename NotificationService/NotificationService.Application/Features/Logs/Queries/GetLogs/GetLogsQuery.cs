using MediatR;
using NotificationService.Domain.Entities;


namespace NotificationService.Application.Features.Logs.Queries.GetLogs
{
    public record GetLogsQuery() : IRequest<List<NotificationLog>>;
}
