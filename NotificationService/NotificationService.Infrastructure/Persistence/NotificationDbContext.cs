
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
namespace NotificationService.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> Templates { get; set; }
        public DbSet<NotificationLog> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>().HasKey(x => x.Id);
            modelBuilder.Entity<NotificationTemplate>().HasKey(x => x.Id);
            modelBuilder.Entity<NotificationLog>().HasKey(x => x.Id);
        }
    }
}
