using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Features.Notifications.Commands.SendNotification
{
    using MediatR;
    using NotificationService.Application.Common.Interfaces;
    using NotificationService.Domain.Entities;
    using NotificationService.Domain.Enums;

    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailService _emailService;

        public SendNotificationHandler(IUnitOfWork uow, IEmailService emailService)
        {
            _uow = uow;
            _emailService = emailService;
        }

        public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                RecipientId = request.RecipientId
            };

            await _uow.Notifications.AddAsync(notification);

            if (request.Type == NotificationType.Email)
            {
                try
                {
                    await _emailService.SendEmailAsync("test@mail.com", request.Title, request.Message);
                    notification.SentAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    await _uow.Logs.AddAsync(new NotificationLog
                    {
                        NotificationId = notification.Id,
                        Status = "Failed",
                        ErrorMessage = ex.Message,
                        SentAt = DateTime.UtcNow
                    });
                }
            }

            await _uow.SaveChangesAsync();
            return notification.Id;
        }
    }
}
