using AutoMapper;
using MediatR;
using NotificationService.Application.Common.DTOs;
using NotificationService.Application.Common.Interfaces;


namespace NotificationService.Application.Features.Notifications.Queries.GetUserNotification
{
    public class GetUserNotificationsHandler : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetUserNotificationsHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var data = await _uow.Notifications.GetAllAsync();
            var filtered = data.Where(x => x.RecipientId == request.UserId).ToList();

            return _mapper.Map<List<NotificationDto>>(filtered);
        }
    }
}
