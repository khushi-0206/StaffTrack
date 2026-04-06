using TimeSheetService.Application.Common.Constants;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Application.Features.Timesheets.Commands.ApproveTimesheet;
using TimeSheetService.Application.Features.Timesheets.Commands.CreateTimesheet;
using TimeSheetService.Application.Features.Timesheets.Commands.DeleteTimesheet;
using TimeSheetService.Application.Features.Timesheets.Commands.RejectTimesheet;
using TimeSheetService.Application.Features.Timesheets.Commands.SubmitTimesheet;
using TimeSheetService.Application.Features.Timesheets.Commands.UpdateTimesheet;
using TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetById;
using TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheets;
using TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetsByEmployee;
using TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetsForManager;
using TimeSheetService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeSheetService.API.Controllers;

[ApiController]
[Route("api/timesheets")]
[Authorize]
public class TimesheetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimesheetsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Create([FromBody] CreateTimesheetRequestDto request)
    {
        var id = await _mediator.Send(new CreateTimesheetCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] TimesheetStatus? status = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var result = await _mediator.Send(new GetTimesheetsQuery(
            page, pageSize, employeeId, status, fromDate, toDate, sortBy, sortDescending));
        return Ok(result);
    }

    [HttpGet("employee/{employeeId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId) =>
        Ok(await _mediator.Send(new GetTimesheetsByEmployeeQuery(employeeId)));

    [HttpGet("manager/{managerId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetForManager(Guid managerId) =>
        Ok(await _mediator.Send(new GetTimesheetsForManagerQuery(managerId)));

    [HttpGet("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetTimesheetByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimesheetRequestDto request)
    {
        await _mediator.Send(new UpdateTimesheetCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTimesheetCommand(id));
        return NoContent();
    }

    [HttpPut("{id:guid}/submit")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Submit(Guid id)
    {
        await _mediator.Send(new SubmitTimesheetCommand(id));
        return NoContent();
    }

    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager}")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveTimesheetRequestDto? dto)
    {
        await _mediator.Send(new ApproveTimesheetCommand(id, dto ?? new ApproveTimesheetRequestDto(null)));
        return NoContent();
    }

    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager}")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectTimesheetRequestDto? dto)
    {
        await _mediator.Send(new RejectTimesheetCommand(id, dto ?? new RejectTimesheetRequestDto(null)));
        return NoContent();
    }
}
