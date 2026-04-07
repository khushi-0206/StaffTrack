using TimeSheetService.Application.Common.Constants;
using TimeSheetService.Application.Features.Reports.Queries.GetEmployeeTimesheetReport;
using TimeSheetService.Application.Features.Reports.Queries.GetMonthlyTimesheetReport;
using TimeSheetService.Application.Features.Reports.Queries.GetProductivityReport;
using TimeSheetService.Application.Features.Reports.Queries.GetWeeklyTimesheetReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeSheetService.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("employee/{employeeId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Employee(Guid employeeId) =>
        Ok(await _mediator.Send(new GetEmployeeTimesheetReportQuery(employeeId)));

    [HttpGet("weekly")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Weekly([FromQuery] int year, [FromQuery] int week) =>
        Ok(await _mediator.Send(new GetWeeklyTimesheetReportQuery(year, week)));

    [HttpGet("monthly")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Monthly([FromQuery] int year, [FromQuery] int month) =>
        Ok(await _mediator.Send(new GetMonthlyTimesheetReportQuery(year, month)));

    [HttpGet("productivity")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> Productivity([FromQuery] DateOnly from, [FromQuery] DateOnly to) =>
        Ok(await _mediator.Send(new GetProductivityReportQuery(from, to)));
}
