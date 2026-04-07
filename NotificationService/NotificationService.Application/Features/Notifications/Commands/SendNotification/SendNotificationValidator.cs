using FluentValidation;

namespace NotificationService.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationValidator : AbstractValidator<SendNotificationCommand>
    {
        public SendNotificationValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Message).NotEmpty();
            RuleFor(x => x.RecipientId).NotEmpty();
        }
    }
}
