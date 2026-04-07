using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Entities
{
    public class NotificationLog
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
    }
}
