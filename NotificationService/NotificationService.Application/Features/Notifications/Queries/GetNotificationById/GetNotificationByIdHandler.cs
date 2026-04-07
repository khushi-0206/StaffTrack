using AutoMapper;
using MediatR;
using NotificationService.Application.Common.DTOs;
using NotificationService.Application.Common.Interfaces;


namespace NotificationService.Application.Features.Notifications.Queries.GetNotificationById
{
    public class GetNotificationByIdHandler : IRequestHandler<GetNotificationByIdQuery, NotificationDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetNotificationByIdHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<NotificationDto> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
            var notification = await _uow.Notifications.GetByIdAsync(request.Id);

            if (notification == null)
                throw new Exception("Notification not found");

            return _mapper.Map<NotificationDto>(notification);
        }
    }
}
