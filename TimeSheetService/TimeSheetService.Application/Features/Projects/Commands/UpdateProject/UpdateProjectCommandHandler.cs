using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;

    public UpdateProjectCommandHandler(IUnitOfWork uow, ICurrentUserService current)
    {
        _uow = uow;
        _current = current;
    }

    public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException();

        var p = await _uow.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (p is null || p.IsDeleted)
            throw new NotFoundException("Project not found.");

        if (await _uow.Projects.NameExistsAsync(request.Dto.Name, request.Id, cancellationToken))
            throw new ConflictException("A project with this name already exists.");

        p.Name = request.Dto.Name.Trim();
        p.Description = request.Dto.Description?.Trim();
        _uow.Projects.Update(p);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
