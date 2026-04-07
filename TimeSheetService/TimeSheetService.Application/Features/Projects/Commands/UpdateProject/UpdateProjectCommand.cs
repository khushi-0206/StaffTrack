using TimeSheetService.Application.DTOs.Projects;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(int Id, UpdateProjectRequestDto Dto) : IRequest<Unit>;
