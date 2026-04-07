using MediatR;
using NotificationService.Application.Common.Interfaces;


namespace NotificationService.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkNotificationReadHandler : IRequestHandler<MarkNotificationReadCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public MarkNotificationReadHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _uow.Notifications.GetByIdAsync(request.Id);

            if (notification == null) return false;

            notification.IsRead = true;
            _uow.Notifications.Update(notification);

            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
