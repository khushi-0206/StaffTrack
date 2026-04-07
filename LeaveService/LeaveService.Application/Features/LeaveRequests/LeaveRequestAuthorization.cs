using LeaveService.Application.Common;
using LeaveService.Application.Common.Constants;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.External;
using LeaveService.Application.Interfaces;
using LeaveService.Domain.Entities;

namespace LeaveService.Application.Features.LeaveRequests;

internal static class LeaveRequestAuthorization
{
    public static async Task<Guid> GetCallerEmployeeIdAsync(
        IEmployeeServiceClient employeeClient,
        ICurrentUserService current,
        CancellationToken cancellationToken)
    {
        var self = await employeeClient.GetSelfEmployeeAsync(cancellationToken);
        if (self is null)
            throw new ForbiddenAppException("No employee profile linked to this account. Ensure JWT email matches an employee record.");
        return self.Id;
    }

    public static void EnsureCanViewEmployeeLeaves(
        ICurrentUserService current,
        Guid targetEmployeeId,
        Guid callerEmployeeId,
        IReadOnlyList<Guid> teamMemberIds)
    {
        if (LeaveRoles.IsHrOrAdmin(current.Roles))
            return;

        if (targetEmployeeId == callerEmployeeId)
            return;

        if (LeaveRoles.IsManager(current.Roles) && teamMemberIds.Contains(targetEmployeeId))
            return;

        throw new ForbiddenAppException("You cannot view leave history for this employee.");
    }

    public static void EnsureCanApplyFor(
        ICurrentUserService current,
        Guid targetEmployeeId,
        Guid callerEmployeeId)
    {
        if (LeaveRoles.IsHrOrAdmin(current.Roles))
            return;

        if (targetEmployeeId == callerEmployeeId)
            return;

        throw new ForbiddenAppException("You can only apply leave for yourself.");
    }

    public static async Task EnsureCanApproveOrRejectAsync(
        ICurrentUserService current,
        LeaveRequest request,
        EmployeeApiProfile subjectEmployee,
        Guid callerEmployeeId,
        CancellationToken cancellationToken)
    {
        if (LeaveRoles.IsHrOrAdmin(current.Roles))
            return;

        if (!LeaveRoles.IsManager(current.Roles))
            throw new ForbiddenAppException("Only a manager or HR can process this request.");

        if (subjectEmployee.ManagerId != callerEmployeeId)
            throw new ForbiddenAppException("Only the employee's assigned manager can approve or reject this request.");
    }

    public static void EnsureCanCancel(
        ICurrentUserService current,
        LeaveRequest request,
        Guid callerEmployeeId)
    {
        if (LeaveRoles.IsHrOrAdmin(current.Roles))
            return;

        if (request.EmployeeId == callerEmployeeId)
            return;

        throw new ForbiddenAppException("You can only cancel your own leave requests.");
    }
}
