using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;


namespace NotificationService.Application.Features.Templates.Commands.CreateTemplate
{
    public class CreateTemplateHandler : IRequestHandler<CreateTemplateCommand, Guid>
    {
        private readonly ITemplateRepository _repository;

        public CreateTemplateHandler(ITemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new Template
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(template);
        }
    }
}
