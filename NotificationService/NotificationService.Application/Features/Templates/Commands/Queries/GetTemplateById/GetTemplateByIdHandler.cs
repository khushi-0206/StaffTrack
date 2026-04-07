using MediatR;
using NotificationService.Application.Common.DTOs;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Application.Features.Templates.Commands.Queries.GetTemplateById
{
    public class GetTemplateByIdHandler : IRequestHandler<GetTemplateByIdQuery, TemplateDto?>
    {
        private readonly ITemplateRepository _repository;

        public GetTemplateByIdHandler(ITemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<TemplateDto?> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id);

            if (template == null) return null;

            return new TemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Content = template.Content
            };
        }
    }
}
