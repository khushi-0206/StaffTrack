using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Commands.UpdateLeaveType;

public class UpdateLeaveTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;

    public UpdateLeaveTypeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _current = current;
    }

    public async Task<Unit> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException();

        var entity = await _unitOfWork.LeaveTypes.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Leave type not found.");

        if (await _unitOfWork.LeaveTypes.NameExistsAsync(request.Dto.Name, request.Id, cancellationToken))
            throw new ConflictException("A leave type with this name already exists.");

        entity.Name = request.Dto.Name.Trim();
        entity.MaxDays = request.Dto.MaxDays;
        _unitOfWork.LeaveTypes.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
