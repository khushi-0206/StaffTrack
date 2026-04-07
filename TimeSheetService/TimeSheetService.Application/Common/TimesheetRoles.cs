using TimeSheetService.Application.Common.Constants;

namespace TimeSheetService.Application.Common;

public static class TimesheetRoles
{
    public static bool IsHrOrAdmin(IEnumerable<string> roles)
    {
        var set = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
        return set.Contains(AuthRoleNames.SystemAdmin)
               || set.Contains(AuthRoleNames.Admin)
               || set.Contains(AuthRoleNames.HR);
    }

    public static bool IsManager(IEnumerable<string> roles) =>
        roles.Contains(AuthRoleNames.Manager, StringComparer.OrdinalIgnoreCase);
}
