using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TimeSheetService.Application.Configuration;
using TimeSheetService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TimeSheetService.Infrastructure.Services;

/// <summary>Reads approved leave from Leave Service to align attendance.</summary>
public class LeaveServiceClient : ILeaveServiceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private const int ApprovedStatus = 1;

    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<LeaveServiceOptions> _options;
    private readonly ILogger<LeaveServiceClient> _logger;

    public LeaveServiceClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IOptions<LeaveServiceOptions> options,
        ILogger<LeaveServiceClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
        _logger = logger;
    }

    public async Task<bool> HasApprovedLeaveOnDateAsync(
        Guid employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Value.BaseUrl))
            return false;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/leaves/employee/{employeeId}");
        if (!TryApplyAuth(request))
            return false;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Leave API returned {Status} for employee {EmployeeId}", (int)response.StatusCode, employeeId);
            return false;
        }

        var rows = await response.Content.ReadFromJsonAsync<List<LeaveRowJson>>(JsonOptions, cancellationToken);
        if (rows is null)
            return false;

        foreach (var row in rows)
        {
            if (row.Status == ApprovedStatus && row.StartDate <= date && row.EndDate >= date)
                return true;
        }

        return false;
    }

    private bool TryApplyAuth(HttpRequestMessage request)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
            return false;
        request.Headers.Authorization = parsed;
        return true;
    }

    private sealed class LeaveRowJson
    {
        public int Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
