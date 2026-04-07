using AuthService.Application.Common.Constants;
using AuthService.Application.Common.Exceptions;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    private readonly IAuthDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _email;
    private readonly ICurrentUserService _currentUser;

    private static readonly HashSet<string> AdminCreatableRoles =
    [
        RoleNames.Employee,
        RoleNames.Manager,
        RoleNames.HR
    ];

    public RegisterHandler(
        IAuthDbContext db,
        IPasswordHasher passwordHasher,
        IEmailService email,
        ICurrentUserService currentUser)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _email = email;
        _currentUser = currentUser;
    }

    public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
        if (exists)
            throw new AppException("A user with this email already exists.");

        var anyUser = await _db.Users.AnyAsync(cancellationToken);

        if (!anyUser)
            return await RegisterBootstrapAsync(request.Request, email, cancellationToken);

        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAppException("Registration is only available for the first user or by Admin/HR.");

        var callerRoles = _currentUser.Roles;
        var canCreate = callerRoles.Contains(RoleNames.SystemAdmin, StringComparer.OrdinalIgnoreCase)
                        || callerRoles.Contains(RoleNames.HR, StringComparer.OrdinalIgnoreCase);
        if (!canCreate)
            throw new ForbiddenAppException("Only System Admin or HR can create users.");

        return await RegisterByAdminAsync(request.Request, email, cancellationToken);
    }

    private async Task<RegisterResponseDto> RegisterBootstrapAsync(
        RegisterRequestDto dto,
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new AppException("Password is required for the first user (System Admin).");

        var adminRole = await _db.Roles.FirstAsync(r => r.Name == RoleNames.SystemAdmin, cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(dto.Password!),
            RoleId = adminRole.Id,
            IsFirstLogin = true,
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Message = "First user registered as System Admin. Sign in and change your password."
        };
    }

    private async Task<RegisterResponseDto> RegisterByAdminAsync(
        RegisterRequestDto dto,
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        var roleName = dto.Role?.Trim();
        if (string.IsNullOrEmpty(roleName) || !AdminCreatableRoles.Contains(roleName))
            throw new AppException($"Role must be one of: {string.Join(", ", AdminCreatableRoles)}.");

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken)
                   ?? throw new AppException("Invalid role.");

        var tempPassword = $"Temp@{Random.Shared.Next(100000, 999999)}";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(tempPassword),
            RoleId = role.Id,
            IsFirstLogin = true,
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        //var body = $"""
        //            Welcome — your account was created.

        //            Email: {user.Email}
        //            Temporary password: {tempPassword}

        //            Sign in and change your password immediately.
        //            """;
        var body = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f6f8;
                                padding: 20px;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: auto;
                                background: white;
                                border-radius: 10px;
                                padding: 20px;
                                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                            }}
                            .header {{
                                background: #4CAF50;
                                color: white;
                                padding: 15px;
                                border-radius: 10px 10px 0 0;
                                text-align: center;
                                font-size: 22px;
                            }}
                            .content {{
                                margin: 20px 0;
                                font-size: 16px;
                            }}
                            .credentials {{
                                background: #f1f1f1;
                                padding: 10px;
                                border-radius: 5px;
                                margin: 10px 0;
                            }}
                            .btn {{
                                display: inline-block;
                                padding: 12px 20px;
                                background: #4CAF50;
                                color: white;
                                text-decoration: none;
                                border-radius: 5px;
                                margin-top: 15px;
                            }}
                            .footer {{
                                font-size: 12px;
                                color: #777;
                                text-align: center;
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>

                    <div class='container'>
                        <div class='header'>
                            StaffTrack Account Created 🎉
                        </div>

                        <div class='content'>
                            <p>Hello <b>{user.Name}</b>,</p>

                            <p>Your account has been successfully created.</p>

                            <div class='credentials'>
                                <p><b>Email:</b> {user.Email}</p>
                                <p><b>Temporary Password:</b> {tempPassword}</p>
                            </div>

                            <p>Please login and change your password immediately.</p>

                            <a href='http://localhost:3000/login' class='btn'>Login Now</a>
                        </div>

                        <div class='footer'>
                            © 2026 StaffTrack | Leave & Time Management System
                        </div>
                    </div>

                    </body>
                    </html>
                    ";

        //await _email.SendEmailAsync(user.Email, "Your account credentials", body, cancellationToken);
        try
        {
            await _email.SendEmailAsync(user.Email, "Your account credentials", body, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error but DO NOT crash
            Console.WriteLine("Email failed: " + ex.Message);
        }

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Message = "User created. Credentials were sent by email."
        };
    }
}
