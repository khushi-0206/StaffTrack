using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Application.Interfaces.Persistence;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);
    Task<int> CountTimeEntryReferencesAsync(int projectId, CancellationToken cancellationToken = default);
    void Add(Project project);
    void Update(Project project);
    void Remove(Project project);
}
