using MediatR;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;


namespace NotificationService.Application.Features.Templates.Commands.UpdateTemplate
{
    public class UpdateTemplateHandler : IRequestHandler<UpdateTemplateCommand, bool>
    {
        private readonly ITemplateRepository _repository;

        public UpdateTemplateHandler(ITemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new Template
            {
                Id = request.Id,
                Name = request.Name,
                Content = request.Content
            };

            return await _repository.UpdateAsync(template);
        }
    }
}
