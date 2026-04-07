using MediatR;
using NotificationService.Application.Common.DTOs;


namespace NotificationService.Application.Features.Templates.Commands.Queries.GetTemplateById
{
    public class GetTemplateByIdQuery : IRequest<TemplateDto?>
    {
        public Guid Id { get; set; }

        public GetTemplateByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
