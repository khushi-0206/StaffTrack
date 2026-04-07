using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _current;

    public DeleteProjectCommandHandler(IUnitOfWork uow, ICurrentUserService current)
    {
        _uow = uow;
        _current = current;
    }

    public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        if (!TimesheetRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException();

        var p = await _uow.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (p is null || p.IsDeleted)
            throw new NotFoundException("Project not found.");

        var refs = await _uow.Projects.CountTimeEntryReferencesAsync(request.Id, cancellationToken);
        if (refs > 0)
            throw new ConflictException("Cannot delete a project referenced by time entries.");

        p.IsDeleted = true;
        _uow.Projects.Update(p);
        await _uow.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
