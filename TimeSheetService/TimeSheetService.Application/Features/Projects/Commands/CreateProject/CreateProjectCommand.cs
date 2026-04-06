using TimeSheetService.Application.DTOs.Projects;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectRequestDto Dto) : IRequest<int>;
