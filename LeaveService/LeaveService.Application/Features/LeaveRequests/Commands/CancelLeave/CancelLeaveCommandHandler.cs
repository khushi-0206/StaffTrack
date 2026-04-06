using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Features.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.CancelLeave;

public class CancelLeaveCommandHandler : IRequestHandler<CancelLeaveCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public CancelLeaveCommandHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(CancelLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _unitOfWork.LeaveRequests.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (leave is null || leave.IsDeleted)
            throw new NotFoundException("Leave request not found.");

        if (leave.Status is LeaveRequestStatus.Rejected or LeaveRequestStatus.Cancelled)
            throw new AppException("This leave request cannot be cancelled.");

        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        LeaveRequestAuthorization.EnsureCanCancel(_current, leave, callerEmployeeId);

        if (leave.Status == LeaveRequestStatus.Approved)
        {
            var days = LeaveCalculation.InclusiveCalendarDays(leave.StartDate, leave.EndDate);
            var balance = await _unitOfWork.LeaveBalances.GetAsync(leave.EmployeeId, leave.LeaveTypeId, cancellationToken);
            if (balance is not null)
            {
                balance.UsedDays = Math.Max(0, balance.UsedDays - days);
                balance.RemainingDays = balance.TotalDays - balance.UsedDays;
                balance.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.LeaveBalances.Update(balance);
            }
        }

        leave.Status = LeaveRequestStatus.Cancelled;
        leave.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.LeaveRequests.Update(leave);

        _unitOfWork.LeaveHistories.Add(new LeaveHistory
        {
            LeaveRequestId = leave.Id,
            Action = LeaveHistoryAction.Cancelled,
            ActionBy = callerEmployeeId,
            ActionDate = DateTime.UtcNow,
            Remarks = request.Dto.Remarks?.Trim()
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
