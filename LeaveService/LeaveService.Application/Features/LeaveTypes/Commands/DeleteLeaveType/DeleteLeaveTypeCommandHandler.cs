using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Commands.DeleteLeaveType;

public class DeleteLeaveTypeCommandHandler : IRequestHandler<DeleteLeaveTypeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;

    public DeleteLeaveTypeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _current = current;
    }

    public async Task<Unit> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException();

        var entity = await _unitOfWork.LeaveTypes.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Leave type not found.");

        var requests = await _unitOfWork.LeaveRequests.CountByLeaveTypeAsync(request.Id, cancellationToken);
        var balances = await _unitOfWork.LeaveBalances.CountByLeaveTypeAsync(request.Id, cancellationToken);
        if (requests > 0 || balances > 0)
            throw new ConflictException("Cannot delete a leave type that is referenced by leave requests or balances.");

        _unitOfWork.LeaveTypes.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
