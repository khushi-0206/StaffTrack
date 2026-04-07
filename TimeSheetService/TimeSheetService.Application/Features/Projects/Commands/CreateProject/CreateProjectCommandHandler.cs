using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;

    public CreateProjectCommandHandler(IUnitOfWork uow, ICurrentUserService current)
    {
        _uow = uow;
        _current = current;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException();

        if (await _uow.Projects.NameExistsAsync(request.Dto.Name, null, cancellationToken))
            throw new ConflictException("A project with this name already exists.");

        var p = new Project { Name = request.Dto.Name.Trim(), Description = request.Dto.Description?.Trim() };
        _uow.Projects.Add(p);
        await _uow.SaveChangesAsync(cancellationToken);
        return p.Id;
    }
}
