using LeaveService.Application.Common.Constants;
using LeaveService.Application.DTOs.LeaveTypes;
using LeaveService.Application.Features.LeaveTypes.Commands.CreateLeaveType;
using LeaveService.Application.Features.LeaveTypes.Commands.DeleteLeaveType;
using LeaveService.Application.Features.LeaveTypes.Commands.UpdateLeaveType;
using LeaveService.Application.Features.LeaveTypes.Queries.GetLeaveTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveService.API.Controllers;

[ApiController]
[Route("api/leavetypes")]
[Authorize]
public class LeaveTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetLeaveTypesQuery());
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Create([FromBody] CreateLeaveTypeRequestDto request)
    {
        var id = await _mediator.Send(new CreateLeaveTypeCommand(request));
        return CreatedAtAction(nameof(GetAll), null, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLeaveTypeRequestDto request)
    {
        await _mediator.Send(new UpdateLeaveTypeCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteLeaveTypeCommand(id));
        return NoContent();
    }
}
