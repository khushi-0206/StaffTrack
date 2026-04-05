using EmployeeService.Application.Common.Constants;
using EmployeeService.Application.DTOs.Roles;
using EmployeeService.Application.Features.Roles.Commands.CreateRole;
using EmployeeService.Application.Features.Roles.Queries.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetRolesQuery());
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request)
    {
        var id = await _mediator.Send(new CreateRoleCommand(request));
        return CreatedAtAction(nameof(GetAll), null, new { id });
    }
}
