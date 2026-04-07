using TimeSheetService.Application.Common.Constants;
using TimeSheetService.Application.DTOs.TimeEntries;
using TimeSheetService.Application.Features.TimeEntries.Commands.CreateTimeEntry;
using TimeSheetService.Application.Features.TimeEntries.Commands.DeleteTimeEntry;
using TimeSheetService.Application.Features.TimeEntries.Commands.UpdateTimeEntry;
using TimeSheetService.Application.Features.TimeEntries.Queries.GetTimeEntriesByTimesheet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeSheetService.API.Controllers;

[ApiController]
[Route("api/timeentries")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimeEntriesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Create([FromBody] CreateTimeEntryRequestDto request)
    {
        var id = await _mediator.Send(new CreateTimeEntryCommand(request));
        return CreatedAtAction(nameof(GetByTimesheet), new { timesheetId = request.TimesheetId }, new { id });
    }

    [HttpGet("timesheet/{timesheetId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByTimesheet(Guid timesheetId) =>
        Ok(await _mediator.Send(new GetTimeEntriesByTimesheetQuery(timesheetId)));

    [HttpPut("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimeEntryRequestDto request)
    {
        await _mediator.Send(new UpdateTimeEntryCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTimeEntryCommand(id));
        return NoContent();
    }
}
