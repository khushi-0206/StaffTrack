using LeaveService.Application.Common.Constants;
using LeaveService.Application.DTOs.LeaveBalance;
using LeaveService.Application.Features.LeaveBalance.Commands.UpdateLeaveBalance;
using LeaveService.Application.Features.LeaveBalance.Queries.GetLeaveBalancesByEmployee;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveService.API.Controllers;

[ApiController]
[Route("api/leavebalance")]
[Authorize]
public class LeaveBalanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveBalanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{employeeId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId)
    {
        var list = await _mediator.Send(new GetLeaveBalancesByEmployeeQuery(employeeId));
        return Ok(list);
    }

    [HttpPut("update")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Update([FromBody] UpdateLeaveBalanceRequestDto request)
    {
        await _mediator.Send(new UpdateLeaveBalanceCommand(request));
        return NoContent();
    }
}
