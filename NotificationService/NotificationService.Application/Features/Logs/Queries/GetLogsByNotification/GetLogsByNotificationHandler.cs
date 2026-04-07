using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Logs.Queries.GetLogsByNotification
{
    public class GetLogsByNotificationHandler : IRequestHandler<GetLogsByNotificationQuery, List<NotificationLog>>
    {
        private readonly IUnitOfWork _uow;

        public GetLogsByNotificationHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<NotificationLog>> Handle(GetLogsByNotificationQuery request, CancellationToken cancellationToken)
        {
            var logs = await _uow.Logs.GetAllAsync();

            return logs
                .Where(x => x.NotificationId == request.NotificationId)
                .ToList();
        }
    }
}
