using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IAuthDbContext _db;
    private readonly IEmailService _email;
    private readonly IConfiguration _configuration;

    public ForgotPasswordHandler(IAuthDbContext db, IEmailService email, IConfiguration configuration)
    {
        _db = db;
        _email = email;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken);

        // Do not reveal whether the account exists
        if (user is null)
            return Unit.Value;

        var minutes = _configuration.GetValue("Security:OtpExpiryMinutes", 15);
        var code = Random.Shared.Next(100000, 999999).ToString();

        var otp = new Otp
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Code = code,
            ExpiryTime = DateTime.UtcNow.AddMinutes(minutes),
            IsUsed = false
        };
        _db.Otps.Add(otp);
        await _db.SaveChangesAsync(cancellationToken);

        var body = $"Your password reset code is: {code}\n\nIt expires in {minutes} minutes.";
        await _email.SendEmailAsync(user.Email, "Password reset code", body, cancellationToken);

        return Unit.Value;
    }
}
