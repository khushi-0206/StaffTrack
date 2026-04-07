using AuthService.Application.Common.Constants;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Helpers;

internal static class TestUsers
{
    public static async Task<User> AddUserAsync(
        IAuthDbContext db,
        IPasswordHasher hasher,
        string email,
        string plainPassword,
        string roleName,
        bool isActive = true,
        bool isFirstLogin = false)
    {
        var role = await db.Roles.FirstAsync(r => r.Name == roleName);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = hasher.Hash(plainPassword),
            RoleId = role.Id,
            IsActive = isActive,
            IsFirstLogin = isFirstLogin
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    public static async Task<User> AddAdminAsync(IAuthDbContext db, IPasswordHasher hasher, string email, string password)
        => await AddUserAsync(db, hasher, email, password, RoleNames.SystemAdmin);

    public static MockCurrentUser MockSystemAdmin(Guid userId) =>
        new(true, userId, new[] { RoleNames.SystemAdmin });

    public static MockCurrentUser MockHr(Guid userId) =>
        new(true, userId, new[] { RoleNames.HR });

    public static MockCurrentUser MockEmployeeCaller(Guid userId) =>
        new(true, userId, new[] { RoleNames.Employee });

    public static MockCurrentUser Anonymous() => new(false, null, Array.Empty<string>());
}

/// <summary>
/// Simple test double for <see cref="ICurrentUserService"/> (replaces Moq for role lists).
/// </summary>
internal sealed class MockCurrentUser : ICurrentUserService
{
    public MockCurrentUser(bool isAuthenticated, Guid? userId, IReadOnlyList<string> roles)
    {
        IsAuthenticated = isAuthenticated;
        UserId = userId;
        Roles = roles;
    }

    public Guid? UserId { get; }
    public bool IsAuthenticated { get; }
    public IReadOnlyList<string> Roles { get; }
}
