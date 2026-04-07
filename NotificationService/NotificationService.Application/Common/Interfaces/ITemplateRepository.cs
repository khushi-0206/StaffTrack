using NotificationService.Domain.Entities;


namespace NotificationService.Application.Common.Interfaces
{
    public interface ITemplateRepository
    {
        Task<Guid> CreateAsync(Template template);
        Task<List<Template>> GetAllAsync();
        Task<Template?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Template template);
        Task<bool> DeleteAsync(Guid id);
    }
}
