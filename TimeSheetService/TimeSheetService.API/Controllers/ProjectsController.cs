using TimeSheetService.Application.Common.Constants;
using TimeSheetService.Application.DTOs.Projects;
using TimeSheetService.Application.Features.Projects.Commands.CreateProject;
using TimeSheetService.Application.Features.Projects.Commands.DeleteProject;
using TimeSheetService.Application.Features.Projects.Commands.UpdateProject;
using TimeSheetService.Application.Features.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TimeSheetService.API.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles =
        $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR},{AuthRoleNames.Manager},{AuthRoleNames.Employee}")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _mediator.Send(new GetProjectsQuery()));

    [HttpPost]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequestDto request)
    {
        var id = await _mediator.Send(new CreateProjectCommand(request));
        return CreatedAtAction(nameof(GetAll), null, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequestDto request)
    {
        await _mediator.Send(new UpdateProjectCommand(id, request));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{AuthRoleNames.SystemAdmin},{AuthRoleNames.Admin},{AuthRoleNames.HR}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteProjectCommand(id));
        return NoContent();
    }
}
