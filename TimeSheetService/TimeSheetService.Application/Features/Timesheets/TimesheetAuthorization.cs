using TimeSheetService.Application.Common;
using TimeSheetService.Application.Common.Constants;
using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.External;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Domain.Entities;

namespace TimeSheetService.Application.Features.Timesheets;

internal static class TimesheetAuthorization
{
    public static async Task<Guid> GetCallerEmployeeIdAsync(
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        CancellationToken cancellationToken)
    {
        var self = await employees.GetSelfEmployeeAsync(cancellationToken);
        if (self is null)
            throw new ForbiddenAppException("No employee profile linked to this account. Ensure JWT email matches an employee record.");
        return self.Id;
    }

    public static void EnsureCanViewEmployeeTimesheets(
        ICurrentUserService current,
        Guid targetEmployeeId,
        Guid callerEmployeeId,
        IReadOnlyList<Guid> teamMemberIds)
    {
        if (TimesheetRoles.IsHrOrAdmin(current.Roles))
            return;
        if (targetEmployeeId == callerEmployeeId)
            return;
        if (TimesheetRoles.IsManager(current.Roles) && teamMemberIds.Contains(targetEmployeeId))
            return;
        throw new ForbiddenAppException("You cannot view timesheets for this employee.");
    }

    public static void EnsureCanCreateFor(
        ICurrentUserService current,
        Guid targetEmployeeId,
        Guid callerEmployeeId)
    {
        if (TimesheetRoles.IsHrOrAdmin(current.Roles))
            return;
        if (targetEmployeeId == callerEmployeeId)
            return;
        throw new ForbiddenAppException("You can only create timesheets for yourself.");
    }

    public static void EnsureCanModifyEntry(
        ICurrentUserService current,
        Timesheet timesheet,
        Guid callerEmployeeId)
    {
        if (TimesheetRoles.IsHrOrAdmin(current.Roles))
            return;
        if (timesheet.EmployeeId == callerEmployeeId && timesheet.Status == Domain.Enums.TimesheetStatus.Pending)
            return;
        throw new ForbiddenAppException("You cannot modify entries for this timesheet.");
    }

    public static async Task EnsureCanApproveOrRejectAsync(
        ICurrentUserService current,
        Timesheet sheet,
        EmployeeApiProfile subjectEmployee,
        Guid callerEmployeeId,
        CancellationToken cancellationToken)
    {
        if (TimesheetRoles.IsHrOrAdmin(current.Roles))
            return;
        if (!TimesheetRoles.IsManager(current.Roles))
            throw new ForbiddenAppException("Only a manager or HR can process this timesheet.");
        if (subjectEmployee.ManagerId != callerEmployeeId)
            throw new ForbiddenAppException("Only the employee's assigned manager can approve or reject this timesheet.");
    }
}
