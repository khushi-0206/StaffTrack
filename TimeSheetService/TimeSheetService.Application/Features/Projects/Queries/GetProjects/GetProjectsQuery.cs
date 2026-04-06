using TimeSheetService.Application.DTOs.Projects;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery : IRequest<IReadOnlyList<ProjectResponseDto>>;
