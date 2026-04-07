using MediatR;


namespace NotificationService.Application.Features.Templates.Commands.UpdateTemplate
{
    public class UpdateTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
