using NotificationService.Domain.Enums;
namespace NotificationService.Domain.Entities
{
    public class NotificationTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public NotificationType Type { get; set; }
    }
}
