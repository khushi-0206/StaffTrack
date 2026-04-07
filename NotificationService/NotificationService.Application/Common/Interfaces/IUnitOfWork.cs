using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<NotificationTemplate> Templates { get; }
        IGenericRepository<NotificationLog> Logs { get; }

        Task<int> SaveChangesAsync();
    }
}
