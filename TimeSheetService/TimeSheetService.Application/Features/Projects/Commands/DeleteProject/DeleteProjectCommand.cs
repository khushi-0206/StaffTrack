using MediatR;

namespace TimeSheetService.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(int Id) : IRequest<Unit>;
