using System.Net.Http.Headers;
using LeaveService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LeaveService.Infrastructure.Services;

public class AuthIntegrationClient : IAuthIntegrationClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthIntegrationClient> _logger;

    public AuthIntegrationClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthIntegrationClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<bool> ValidateCallerAsync(CancellationToken cancellationToken = default)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
        {
            _logger.LogWarning("No Authorization header to validate with Auth Service.");
            return false;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
        request.Headers.Authorization = parsed;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            _logger.LogWarning("Auth Service validation returned {StatusCode}", (int)response.StatusCode);

        return response.IsSuccessStatusCode;
    }
}
