using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NotificationDbContext _context;

        public IGenericRepository<Notification> Notifications { get; }
        public IGenericRepository<NotificationTemplate> Templates { get; }
        public IGenericRepository<NotificationLog> Logs { get; }

        public UnitOfWork(NotificationDbContext context)
        {
            _context = context;

            Notifications = new GenericRepository<Notification>(_context);
            Templates = new GenericRepository<NotificationTemplate>(_context);
            Logs = new GenericRepository<NotificationLog>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
