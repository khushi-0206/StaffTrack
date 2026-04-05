using Microsoft.Extensions.Configuration;

namespace AuthService.Tests.Helpers;

public static class TestConfigurationFactory
{
    /// <summary>
    /// JWT + security settings compatible with <see cref="Infrastructure.Services.JwtService"/> and handlers.
    /// </summary>
    public static IConfiguration Create(
        int? maxFailedAttempts = null,
        int? lockoutMinutes = null,
        int? otpExpiryMinutes = null)
    {
        var config = new ConfigurationManager();
        config["Jwt:Key"] = "UNIT_TEST_JWT_KEY_MUST_BE_LONG_ENOUGH_32_CHARS!";
        config["Jwt:Issuer"] = "TestIssuer";
        config["Jwt:Audience"] = "TestAudience";
        config["Jwt:AccessTokenMinutes"] = "15";
        config["Jwt:RefreshTokenExpiryDays"] = "7";
        config["Security:MaxFailedAttempts"] = (maxFailedAttempts ?? 5).ToString();
        config["Security:LockoutMinutes"] = (lockoutMinutes ?? 15).ToString();
        config["Security:OtpExpiryMinutes"] = (otpExpiryMinutes ?? 15).ToString();
        return config;
    }
}
