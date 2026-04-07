using EmployeeService.Application.Common.Constants;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Features.Employees.Commands.CreateEmployee;
using EmployeeService.Application.Features.Employees.Commands.SoftDeleteEmployee;
using EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;
using EmployeeService.Application.Features.Employees.Queries.GetEmployeeById;
using EmployeeService.Application.Features.Employees.Queries.GetEmployeeSelf;
using EmployeeService.Application.Features.Employees.Queries.GetEmployees;
using EmployeeService.Application.Features.Employees.Queries.GetEmployeesByDepartment;
using EmployeeService.Application.Features.Employees.Queries.GetEmployeesByManager;
using EmployeeService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>List employees with pagination, filters, and search (name/email).</summary>
    [HttpGet]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] int? roleId = null,
        [FromQuery] EmployeeStatus? status = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false)
    {
        var result = await _mediator.Send(new GetEmployeesQuery(
            page, pageSize, search, departmentId, roleId, status, sortBy, sortDescending));
        return Ok(result);
    }

    [HttpGet("department/{departmentId:int}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
    {
        var list = await _mediator.Send(new GetEmployeesByDepartmentQuery(departmentId));
        return Ok(list);
    }

    [HttpGet("manager/{managerId:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetByManager(Guid managerId)
    {
        var list = await _mediator.Send(new GetEmployeesByManagerQuery(managerId));
        return Ok(list);
    }

    /// <summary>Current user's employee record (JWT email must match Employee.Email). Used by Leave Service for manager checks.</summary>
    [HttpGet("self")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetSelf()
    {
        var dto = await _mediator.Send(new GetEmployeeSelfQuery());
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetEmployeeByIdQuery(id));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequestDto request)
    {
        var id = await _mediator.Send(new CreateEmployeeCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequestDto request)
    {
        await _mediator.Send(new UpdateEmployeeCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        await _mediator.Send(new SoftDeleteEmployeeCommand(id));
        return NoContent();
    }
}
