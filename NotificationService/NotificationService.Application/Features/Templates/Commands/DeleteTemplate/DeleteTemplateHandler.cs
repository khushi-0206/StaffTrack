using MediatR;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Application.Features.Templates.Commands.DeleteTemplate
{
    public class DeleteTemplateHandler : IRequestHandler<DeleteTemplateCommand, bool>
    {
        private readonly ITemplateRepository _repository;

        public DeleteTemplateHandler(ITemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Id);
        }
    }
}
