using TimeSheetService.Application.Common.Constants;
using TimeSheetService.Application.DTOs.Attendance;
using TimeSheetService.Application.Features.Attendance.Commands.CheckIn;
using TimeSheetService.Application.Features.Attendance.Commands.CheckOut;
using TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceByDate;
using TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceByEmployee;
using TimeSheetService.Application.Features.Attendance.Queries.GetAttendanceReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeSheetService.API.Controllers;

[ApiController]
[Route("api/attendance")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceController(IMediator mediator) => _mediator = mediator;

    [HttpPost("checkin")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
    {
        var id = await _mediator.Send(new CheckInCommand(request));
        return Ok(new { id });
    }

    [HttpPost("checkout")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequestDto request)
    {
        await _mediator.Send(new CheckOutCommand(request));
        return NoContent();
    }

    [HttpGet("{employeeId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByEmployee(
        Guid employeeId,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null) =>
        Ok(await _mediator.Send(new GetAttendanceByEmployeeQuery(employeeId, from, to)));

    [HttpGet("date/{date}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> GetByDate([FromRoute] DateOnly date) =>
        Ok(await _mediator.Send(new GetAttendanceByDateQuery(date)));

    [HttpGet("report")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Report(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50) =>
        Ok(await _mediator.Send(new GetAttendanceReportQuery(from, to, employeeId, page, pageSize)));
}
