using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveBalanceEntity = LeaveService.Domain.Entities.LeaveBalance;
using MediatR;

namespace LeaveService.Application.Features.LeaveBalance.Commands.UpdateLeaveBalance;

public class UpdateLeaveBalanceCommandHandler : IRequestHandler<UpdateLeaveBalanceCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;

    public UpdateLeaveBalanceCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _current = current;
    }

    public async Task<Unit> Handle(UpdateLeaveBalanceCommand request, CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException("Only HR or administrators can adjust leave balances.");

        var lt = await _unitOfWork.LeaveTypes.GetByIdAsync(request.Dto.LeaveTypeId, cancellationToken);
        if (lt is null)
            throw new NotFoundException("Leave type not found.");

        var balance = await _unitOfWork.LeaveBalances.GetAsync(
            request.Dto.EmployeeId, request.Dto.LeaveTypeId, cancellationToken);

        if (balance is null)
        {
            balance = new LeaveBalanceEntity
            {
                EmployeeId = request.Dto.EmployeeId,
                LeaveTypeId = request.Dto.LeaveTypeId,
                LeaveType = lt,
                TotalDays = request.Dto.TotalDays,
                UsedDays = request.Dto.UsedDays,
                RemainingDays = request.Dto.TotalDays - request.Dto.UsedDays,
                UpdatedAt = DateTime.UtcNow
            };
            _unitOfWork.LeaveBalances.Add(balance);
        }
        else
        {
            balance.TotalDays = request.Dto.TotalDays;
            balance.UsedDays = request.Dto.UsedDays;
            balance.RemainingDays = request.Dto.TotalDays - request.Dto.UsedDays;
            balance.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.LeaveBalances.Update(balance);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
