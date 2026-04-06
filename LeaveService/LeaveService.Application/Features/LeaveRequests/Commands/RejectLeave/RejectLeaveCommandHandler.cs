using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Features.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using LeaveService.Domain.Enums;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Commands.RejectLeave;

public class RejectLeaveCommandHandler : IRequestHandler<RejectLeaveCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;

    public RejectLeaveCommandHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
    }

    public async Task<Unit> Handle(RejectLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await _unitOfWork.LeaveRequests.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (leave is null || leave.IsDeleted)
            throw new NotFoundException("Leave request not found.");

        if (leave.Status != LeaveRequestStatus.Pending)
            throw new AppException("Only pending requests can be rejected.");

        var subject = await _employees.GetEmployeeAsync(leave.EmployeeId, cancellationToken);
        if (subject is null)
            throw new NotFoundException("Employee record not found for this request.");

        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        await LeaveRequestAuthorization.EnsureCanApproveOrRejectAsync(
            _current, leave, subject, callerEmployeeId, cancellationToken);

        leave.Status = LeaveRequestStatus.Rejected;
        leave.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.LeaveRequests.Update(leave);

        _unitOfWork.LeaveHistories.Add(new LeaveHistory
        {
            LeaveRequestId = leave.Id,
            Action = LeaveHistoryAction.Rejected,
            ActionBy = callerEmployeeId,
            ActionDate = DateTime.UtcNow,
            Remarks = request.Dto.Remarks?.Trim()
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
