using LeaveService.Application.Common.Constants;
using LeaveService.Application.Features.Reports.Queries.GetDepartmentLeaveReport;
using LeaveService.Application.Features.Reports.Queries.GetEmployeeLeaveReport;
using LeaveService.Application.Features.Reports.Queries.GetMonthlyLeaveReport;
using LeaveService.Application.Features.Reports.Queries.GetYearlyLeaveReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveService.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("employee/{employeeId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> EmployeeReport(Guid employeeId)
    {
        var report = await _mediator.Send(new GetEmployeeLeaveReportQuery(employeeId));
        return Ok(report);
    }

    [HttpGet("department/{departmentId:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> DepartmentReport(
        int departmentId,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null)
    {
        var report = await _mediator.Send(new GetDepartmentLeaveReportQuery(departmentId, from, to));
        return Ok(report);
    }

    [HttpGet("monthly")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Monthly([FromQuery] int year, [FromQuery] int month)
    {
        var report = await _mediator.Send(new GetMonthlyLeaveReportQuery(year, month));
        return Ok(report);
    }

    [HttpGet("yearly")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Yearly([FromQuery] int year)
    {
        var report = await _mediator.Send(new GetYearlyLeaveReportQuery(year));
        return Ok(report);
    }
}
