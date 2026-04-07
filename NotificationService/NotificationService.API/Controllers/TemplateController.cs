using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Features.Templates.Commands.Queries.GetTemplates;
using NotificationService.Application.Features.Templates.Commands.CreateTemplate;
using NotificationService.Application.Features.Templates.Commands.Queries.GetTemplateById;
using NotificationService.Application.Features.Templates.Commands.DeleteTemplate;
using NotificationService.Application.Features.Templates.Commands.UpdateTemplate;
namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateTemplateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetTemplatesQuery());
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetTemplateByIdQuery(id));
            return Ok(result);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTemplateCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteTemplateCommand(id));
            return Ok(result);
        }
    }
}
