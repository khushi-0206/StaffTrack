using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LeaveService.Application.Configuration;
using LeaveService.Application.External;
using LeaveService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LeaveService.Infrastructure.Services;

/// <summary>Forwards the inbound JWT to Employee API and maps JSON to <see cref="EmployeeApiProfile"/>.</summary>
public class EmployeeServiceClient : IEmployeeServiceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<EmployeeServiceOptions> _options;
    private readonly ILogger<EmployeeServiceClient> _logger;

    public EmployeeServiceClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IOptions<EmployeeServiceOptions> options,
        ILogger<EmployeeServiceClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
        _logger = logger;
    }

    public Task<EmployeeApiProfile?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        SendForEmployeeAsync($"api/employees/{employeeId}", cancellationToken);

    public Task<EmployeeApiProfile?> GetSelfEmployeeAsync(CancellationToken cancellationToken = default) =>
        SendForEmployeeAsync("api/employees/self", cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetEmployeeIdsForManagerAsync(
        Guid managerEmployeeId,
        CancellationToken cancellationToken = default)
    {
        var list = await SendForListAsync($"api/employees/manager/{managerEmployeeId}", cancellationToken);
        return list;
    }

    public async Task<IReadOnlyList<Guid>> GetEmployeeIdsForDepartmentAsync(
        int departmentId,
        CancellationToken cancellationToken = default)
    {
        var list = await SendForListAsync($"api/employees/department/{departmentId}", cancellationToken);
        return list;
    }

    private async Task<EmployeeApiProfile?> SendForEmployeeAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Value.BaseUrl))
        {
            _logger.LogWarning("EmployeeService:BaseUrl is not configured.");
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        if (!TryApplyAuthorization(request))
            return null;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Employee API {Url} returned {Status}", relativeUrl, (int)response.StatusCode);
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<EmployeeApiJson>(JsonOptions, cancellationToken);
        return dto is null ? null : Map(dto);
    }

    private async Task<IReadOnlyList<Guid>> SendForListAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Value.BaseUrl))
        {
            _logger.LogWarning("EmployeeService:BaseUrl is not configured.");
            return Array.Empty<Guid>();
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        if (!TryApplyAuthorization(request))
            return Array.Empty<Guid>();

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Employee API {Url} returned {Status}", relativeUrl, (int)response.StatusCode);
            return Array.Empty<Guid>();
        }

        var items = await response.Content.ReadFromJsonAsync<List<EmployeeListItemJson>>(JsonOptions, cancellationToken);
        if (items is null || items.Count == 0)
            return Array.Empty<Guid>();

        return items.Select(x => x.Id).Distinct().ToList();
    }

    private bool TryApplyAuthorization(HttpRequestMessage request)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
        {
            _logger.LogWarning("No Authorization header to call Employee API.");
            return false;
        }

        request.Headers.Authorization = parsed;
        return true;
    }

    private static EmployeeApiProfile Map(EmployeeApiJson j) => new()
    {
        Id = j.Id,
        Email = j.Email ?? "",
        FirstName = j.FirstName ?? "",
        LastName = j.LastName ?? "",
        DepartmentId = j.DepartmentId,
        ManagerId = j.ManagerId
    };

    private sealed class EmployeeApiJson
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
    }

    private sealed class EmployeeListItemJson
    {
        public Guid Id { get; set; }
    }
}
