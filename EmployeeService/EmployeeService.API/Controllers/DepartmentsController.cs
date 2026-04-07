using EmployeeService.Application.Common.Constants;
using EmployeeService.Application.DTOs.Departments;
using EmployeeService.Application.Features.Departments.Commands.CreateDepartment;
using EmployeeService.Application.Features.Departments.Commands.DeleteDepartment;
using EmployeeService.Application.Features.Departments.Commands.UpdateDepartment;
using EmployeeService.Application.Features.Departments.Queries.GetDepartmentById;
using EmployeeService.Application.Features.Departments.Queries.GetDepartments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetDepartmentsQuery());
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _mediator.Send(new GetDepartmentByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequestDto request)
    {
        var id = await _mediator.Send(new CreateDepartmentCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequestDto request)
    {
        await _mediator.Send(new UpdateDepartmentCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteDepartmentCommand(id));
        return NoContent();
    }
}
