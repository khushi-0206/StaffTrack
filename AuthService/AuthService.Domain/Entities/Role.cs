namespace AuthService.Domain.Entities;

/// <summary>
/// Role definition for RBAC (seeded: Employee, Manager, HR, SystemAdmin).
/// </summary>
public class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
