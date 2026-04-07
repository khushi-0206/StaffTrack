using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Features.Notifications.Commands.MarkAsRead;
using NotificationService.Application.Features.Notifications.Commands.SendNotification;
using NotificationService.Application.Features.Notifications.Queries.GetUserNotification;
using NotificationService.Application.Features.Notifications.Queries.GetNotificationById;
using NotificationService.Application.Features.Logs.Queries.GetLogsByNotification;
namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ✅ Send Notification
        [HttpPost("send")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Send(SendNotificationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // ✅ Get User Notifications
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserNotifications(Guid userId)
        {
            var result = await _mediator.Send(new GetUserNotificationsQuery(userId));
            return Ok(result);
        }

        // ✅ Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetNotificationByIdQuery(id));
            return Ok(result);
        }

        // ✅ Mark As Read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var result = await _mediator.Send(new MarkNotificationReadCommand(id));
            return Ok(result);
        }
    }

}
