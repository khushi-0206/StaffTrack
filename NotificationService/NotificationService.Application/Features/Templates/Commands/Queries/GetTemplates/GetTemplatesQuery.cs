using MediatR;
using NotificationService.Application.Common.DTOs;


namespace NotificationService.Application.Features.Templates.Commands.Queries.GetTemplates
{
    public class GetTemplatesQuery : IRequest<List<TemplateDto>>
    {
    }
}
