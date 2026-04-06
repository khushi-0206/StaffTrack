using LeaveService.Application.Common.Constants;

namespace LeaveService.Application.Common;

/// <summary>Maps JWT roles to leave workflow permissions.</summary>
public static class LeaveRoles
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
