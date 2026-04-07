using MediatR;
using NotificationService.Application.Common.DTOs;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Application.Features.Templates.Commands.Queries.GetTemplates
{
    public class GetTemplatesHandler : IRequestHandler<GetTemplatesQuery, List<TemplateDto>>
    {
        private readonly ITemplateRepository _repository;

        public GetTemplatesHandler(ITemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TemplateDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetAllAsync();

            return templates.Select(t => new TemplateDto
            {
                Id = t.Id,
                Name = t.Name,
                Content = t.Content
            }).ToList();
        }
    }
}
