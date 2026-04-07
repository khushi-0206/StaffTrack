using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;


namespace NotificationService.Infrastructure.Repositories
{
    public class TemplateRepository : GenericRepository<NotificationTemplate>, ITemplateRepository
    {
        public TemplateRepository(NotificationDbContext context) : base(context)
        {
        }

        public Task<Guid> CreateAsync(Template template)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Template template)
        {
            throw new NotImplementedException();
        }

        Task<List<Template>> ITemplateRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Template?> ITemplateRepository.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
