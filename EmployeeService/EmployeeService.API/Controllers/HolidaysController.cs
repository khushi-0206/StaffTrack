using EmployeeService.Application.Common.Constants;
using EmployeeService.Application.DTOs.Holidays;
using EmployeeService.Application.Features.Holidays.Commands.CreateHoliday;
using EmployeeService.Application.Features.Holidays.Commands.DeleteHoliday;
using EmployeeService.Application.Features.Holidays.Queries.GetHolidays;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.API.Controllers;

[ApiController]
[Route("api/holidays")]
[Authorize]
public class HolidaysController : ControllerBase
{
    private readonly IMediator _mediator;

    public HolidaysController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetHolidaysQuery());
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Create([FromBody] CreateHolidayRequestDto request)
    {
        var id = await _mediator.Send(new CreateHolidayCommand(request));
        return CreatedAtAction(nameof(GetAll), null, new { id });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteHolidayCommand(id));
        return NoContent();
    }
}
