using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Features.Logs.Queries.GetLogs;
using NotificationService.Application.Features.Logs.Queries.GetLogsByNotification;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(await _mediator.Send(new GetLogsQuery()));
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetLogsByNotification(Guid notificationId)
        {
            return Ok(await _mediator.Send(new GetLogsByNotificationQuery(notificationId)));
        }
    }
}
