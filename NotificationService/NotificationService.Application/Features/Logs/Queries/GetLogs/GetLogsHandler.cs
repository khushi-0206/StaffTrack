using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;


namespace NotificationService.Application.Features.Logs.Queries.GetLogs
{
    public class GetLogsHandler : IRequestHandler<GetLogsQuery, List<NotificationLog>>
    {
        private readonly IUnitOfWork _uow;

        public GetLogsHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<NotificationLog>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _uow.Logs.GetAllAsync();
            return logs.ToList();
        }
    }
}
