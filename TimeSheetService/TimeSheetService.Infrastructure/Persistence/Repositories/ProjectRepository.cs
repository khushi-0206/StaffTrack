using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetService.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TimeSheetDbContext _context;

    public ProjectRepository(TimeSheetDbContext context) => _context = context;

    public Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Projects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Projects.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        var n = name.Trim().ToLowerInvariant();
        var q = _context.Projects.Where(x => !x.IsDeleted && x.Name.ToLower() == n);
        if (excludeId is { } id)
            q = q.Where(x => x.Id != id);
        return q.AnyAsync(cancellationToken);
    }

    public Task<int> CountTimeEntryReferencesAsync(int projectId, CancellationToken cancellationToken = default) =>
        _context.TimeEntries.CountAsync(x => x.ProjectId == projectId, cancellationToken);

    public void Add(Project project) => _context.Projects.Add(project);
    public void Update(Project project) => _context.Projects.Update(project);
    public void Remove(Project project) => _context.Projects.Remove(project);
}
