using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Configuration;
using LeaveService.Application.Features.LeaveRequests.Events;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using LeaveBalanceEntity = LeaveService.Domain.Entities.LeaveBalance;
using LeaveService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace LeaveService.Application.Features.LeaveRequests.Commands.ApplyLeave;

public class ApplyLeaveCommandHandler : IRequestHandler<ApplyLeaveCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IAuthIntegrationClient _authClient;
    private readonly IOptions<AuthServiceOptions> _authOptions;
    private readonly IPublisher _publisher;

    private static readonly LeaveRequestStatus[] BlockingStatuses =
        { LeaveRequestStatus.Pending, LeaveRequestStatus.Approved };

    public ApplyLeaveCommandHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IAuthIntegrationClient authClient,
        IOptions<AuthServiceOptions> authOptions,
        IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
        _authClient = authClient;
        _authOptions = authOptions;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
    {
        if (_authOptions.Value.ValidateWithAuthService
            && !await _authClient.ValidateCallerAsync(cancellationToken))
            throw new ForbiddenAppException("Token could not be validated with Auth Service.");

        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        LeaveRequestAuthorization.EnsureCanApplyFor(
            _current, request.Dto.EmployeeId, callerEmployeeId);

        var employee = await _employees.GetEmployeeAsync(request.Dto.EmployeeId, cancellationToken);
        if (employee is null)
            throw new NotFoundException("Employee was not found in Employee Service.");

        var leaveType = await _unitOfWork.LeaveTypes.GetByIdAsync(request.Dto.LeaveTypeId, cancellationToken);
        if (leaveType is null)
            throw new NotFoundException("Leave type not found.");

        var days = LeaveCalculation.InclusiveCalendarDays(request.Dto.StartDate, request.Dto.EndDate);
        if (days > leaveType.MaxDays)
            throw new AppException($"Requested duration ({days} days) exceeds policy maximum ({leaveType.MaxDays}) for this leave type.");

        var balance = await GetOrCreateBalanceAsync(request.Dto.EmployeeId, leaveType, cancellationToken);
        if (balance.RemainingDays < days)
            throw new AppException($"Insufficient leave balance. Remaining: {balance.RemainingDays}, requested: {days}.");

        if (await _unitOfWork.LeaveRequests.HasOverlappingAsync(
                request.Dto.EmployeeId,
                request.Dto.StartDate,
                request.Dto.EndDate,
                BlockingStatuses,
                null,
                cancellationToken))
            throw new ConflictException("Leave dates overlap an existing pending or approved request.");

        var entity = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.Dto.EmployeeId,
            LeaveTypeId = leaveType.Id,
            LeaveType = leaveType,
            StartDate = request.Dto.StartDate,
            EndDate = request.Dto.EndDate,
            Reason = request.Dto.Reason.Trim(),
            Status = LeaveRequestStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        _unitOfWork.LeaveRequests.Add(entity);
        _unitOfWork.LeaveHistories.Add(new LeaveHistory
        {
            LeaveRequestId = entity.Id,
            Action = LeaveHistoryAction.Created,
            ActionBy = callerEmployeeId,
            ActionDate = DateTime.UtcNow,
            Remarks = null
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new LeaveAppliedNotification(entity.Id), cancellationToken);

        return entity.Id;
    }

    private async Task<LeaveBalanceEntity> GetOrCreateBalanceAsync(
        Guid employeeId,
        LeaveType leaveType,
        CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.LeaveBalances.GetAsync(employeeId, leaveType.Id, cancellationToken);
        if (existing is not null)
            return existing;

        var balance = new LeaveBalanceEntity
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveType.Id,
            LeaveType = leaveType,
            TotalDays = leaveType.MaxDays,
            UsedDays = 0,
            RemainingDays = leaveType.MaxDays,
            UpdatedAt = DateTime.UtcNow
        };

        _unitOfWork.LeaveBalances.Add(balance);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await _unitOfWork.LeaveBalances.GetAsync(employeeId, leaveType.Id, cancellationToken)
               ?? balance;
    }
}
