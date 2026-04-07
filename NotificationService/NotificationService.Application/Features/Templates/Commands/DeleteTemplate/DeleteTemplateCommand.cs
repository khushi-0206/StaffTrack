using MediatR;


namespace NotificationService.Application.Features.Templates.Commands.DeleteTemplate
{
    public class DeleteTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteTemplateCommand(Guid id)
        {
            Id = id;
        }
    }
}
