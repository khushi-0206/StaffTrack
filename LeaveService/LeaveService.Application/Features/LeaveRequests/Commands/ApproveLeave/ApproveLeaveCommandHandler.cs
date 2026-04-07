using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Features.LeaveRequests.Events;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.ApproveLeave;

public class ApproveLeaveCommandHandler : IRequestHandler<ApproveLeaveCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IPublisher _publisher;

    private static readonly LeaveRequestStatus[] BlockingStatuses =
        { LeaveRequestStatus.Pending, LeaveRequestStatus.Approved };

    public ApproveLeaveCommandHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(ApproveLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _unitOfWork.LeaveRequests.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (leave is null || leave.IsDeleted)
            throw new NotFoundException("Leave request not found.");

        if (leave.Status != LeaveRequestStatus.Pending)
            throw new AppException("Only pending requests can be approved.");

        var subject = await _employees.GetEmployeeAsync(leave.EmployeeId, cancellationToken);
        if (subject is null)
            throw new NotFoundException("Employee record not found for this request.");

        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        await LeaveRequestAuthorization.EnsureCanApproveOrRejectAsync(
            _current, leave, subject, callerEmployeeId, cancellationToken);

        var days = LeaveCalculation.InclusiveCalendarDays(leave.StartDate, leave.EndDate);

        if (await _unitOfWork.LeaveRequests.HasOverlappingAsync(
                leave.EmployeeId,
                leave.StartDate,
                leave.EndDate,
                BlockingStatuses,
                leave.Id,
                cancellationToken))
            throw new ConflictException("Another leave overlaps these dates.");

        var balance = await _unitOfWork.LeaveBalances.GetAsync(leave.EmployeeId, leave.LeaveTypeId, cancellationToken);
        if (balance is null || balance.RemainingDays < days)
            throw new AppException("Insufficient leave balance to approve this request.");

        balance.UsedDays += days;
        balance.RemainingDays = balance.TotalDays - balance.UsedDays;
        balance.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.LeaveBalances.Update(balance);

        leave.Status = LeaveRequestStatus.Approved;
        leave.ApprovedByEmployeeId = callerEmployeeId;
        leave.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.LeaveRequests.Update(leave);

        _unitOfWork.LeaveHistories.Add(new LeaveHistory
        {
            LeaveRequestId = leave.Id,
            Action = LeaveHistoryAction.Approved,
            ActionBy = callerEmployeeId,
            ActionDate = DateTime.UtcNow,
            Remarks = request.Dto.Remarks?.Trim()
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new LeaveApprovedNotification(leave.Id), cancellationToken);

        return Unit.Value;
    }
}
