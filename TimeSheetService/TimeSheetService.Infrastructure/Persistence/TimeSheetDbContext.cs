using TimeSheetService.Domain.Common;
using TimeSheetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetService.Infrastructure.Persistence;

public class TimeSheetDbContext : DbContext
{
    public TimeSheetDbContext(DbContextOptions<TimeSheetDbContext> options)
        : base(options)
    {
    }

    public DbSet<Timesheet> Timesheets => Set<Timesheet>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TimesheetHistory> TimesheetHistories => Set<TimesheetHistory>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utc = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = utc;
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = utc;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(2000);
            e.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        modelBuilder.Entity<Timesheet>(e =>
        {
            e.Property(x => x.TotalHours).HasPrecision(12, 2);
            e.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        modelBuilder.Entity<TimeEntry>(e =>
        {
            e.Property(x => x.ProjectName).HasMaxLength(200);
            e.Property(x => x.TaskDescription).HasMaxLength(2000);
            e.Property(x => x.HoursWorked).HasPrecision(8, 2);
            e.HasOne(x => x.Timesheet).WithMany(t => t.Entries).HasForeignKey(x => x.TimesheetId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Project).WithMany(p => p.TimeEntries).HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Attendance>(e =>
        {
            e.Property(x => x.TotalHours).HasPrecision(12, 2);
            e.HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        modelBuilder.Entity<TimesheetHistory>(e =>
        {
            e.Property(x => x.Remarks).HasMaxLength(2000);
            e.HasOne(x => x.Timesheet).WithMany(t => t.Histories).HasForeignKey(x => x.TimesheetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
