

using MediatR;

namespace NotificationService.Application.Features.Templates.Commands.CreateTemplate
{
    public class CreateTemplateCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
