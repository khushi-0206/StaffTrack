using LeaveService.Application.Common.Constants;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Application.Features.LeaveRequests.Commands.ApplyLeave;
using LeaveService.Application.Features.LeaveRequests.Commands.ApproveLeave;
using LeaveService.Application.Features.LeaveRequests.Commands.CancelLeave;
using LeaveService.Application.Features.LeaveRequests.Commands.RejectLeave;
using LeaveService.Application.Features.LeaveRequests.Queries.GetLeaveById;
using LeaveService.Application.Features.LeaveRequests.Queries.GetLeaveRequests;
using LeaveService.Application.Features.LeaveRequests.Queries.GetLeavesByEmployee;
using LeaveService.Application.Features.LeaveRequests.Queries.GetLeavesForManager;
using LeaveService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveService.API.Controllers;

[ApiController]
[Route("api/leaves")]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeavesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Submit a new leave request (employee self-service or HR on behalf).</summary>
    [HttpPost]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Apply([FromBody] ApplyLeaveRequestDto request)
    {
        var id = await _mediator.Send(new ApplyLeaveCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Paginated list of all requests (HR / Admin).</summary>
    [HttpGet]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] LeaveRequestStatus? status = null,
        [FromQuery] int? leaveTypeId = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var result = await _mediator.Send(new GetLeaveRequestsQuery(
            page, pageSize, employeeId, status, leaveTypeId, fromDate, toDate, sortBy, sortDescending));
        return Ok(result);
    }

    [HttpGet("employee/{employeeId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId)
    {
        var list = await _mediator.Send(new GetLeavesByEmployeeQuery(employeeId));
        return Ok(list);
    }

    [HttpGet("manager/{managerId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetForManager(Guid managerId)
    {
        var list = await _mediator.Send(new GetLeavesForManagerQuery(managerId));
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetLeaveByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager}")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveLeaveRequestDto? dto)
    {
        await _mediator.Send(new ApproveLeaveCommand(id, dto ?? new ApproveLeaveRequestDto(null)));
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager}")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectLeaveRequestDto? dto)
    {
        await _mediator.Send(new RejectLeaveCommand(id, dto ?? new RejectLeaveRequestDto(null)));
        return NoContent();
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelLeaveRequestDto? dto)
    {
        await _mediator.Send(new CancelLeaveCommand(id, dto ?? new CancelLeaveRequestDto(null)));
        return NoContent();
    }
}
