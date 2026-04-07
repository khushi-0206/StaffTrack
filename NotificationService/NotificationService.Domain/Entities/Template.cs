

namespace NotificationService.Domain.Entities
{
    public class Template
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
