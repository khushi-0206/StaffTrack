using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TimeSheetService.Application.Configuration;
using TimeSheetService.Application.External;
using TimeSheetService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TimeSheetService.Infrastructure.Services;

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
        GetEmployeeInternalAsync($"api/employees/{employeeId}", cancellationToken);

    public Task<EmployeeApiProfile?> GetSelfEmployeeAsync(CancellationToken cancellationToken = default) =>
        GetEmployeeInternalAsync("api/employees/self", cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetEmployeeIdsForManagerAsync(
        Guid managerEmployeeId,
        CancellationToken cancellationToken = default) =>
        await GetIdListAsync($"api/employees/manager/{managerEmployeeId}", cancellationToken);

    public async Task<IReadOnlyList<Guid>> GetEmployeeIdsForDepartmentAsync(
        int departmentId,
        CancellationToken cancellationToken = default) =>
        await GetIdListAsync($"api/employees/department/{departmentId}", cancellationToken);

    private async Task<EmployeeApiProfile?> GetEmployeeInternalAsync(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Value.BaseUrl))
        {
            _logger.LogWarning("EmployeeService:BaseUrl is not configured.");
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!TryApplyAuth(request))
            return null;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Employee API {Url} returned {Status}", url, (int)response.StatusCode);
            return null;
        }

        var dto = await response.Content.ReadFromJsonAsync<EmployeeJson>(JsonOptions, cancellationToken);
        return dto is null ? null : Map(dto);
    }

    private async Task<IReadOnlyList<Guid>> GetIdListAsync(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Value.BaseUrl))
            return Array.Empty<Guid>();

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!TryApplyAuth(request))
            return Array.Empty<Guid>();

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<Guid>();

        var items = await response.Content.ReadFromJsonAsync<List<EmployeeListJson>>(JsonOptions, cancellationToken);
        if (items is null || items.Count == 0)
            return Array.Empty<Guid>();
        return items.Select(x => x.Id).Distinct().ToList();
    }

    private bool TryApplyAuth(HttpRequestMessage request)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
        {
            _logger.LogWarning("No Authorization header for Employee API.");
            return false;
        }

        request.Headers.Authorization = parsed;
        return true;
    }

    private static EmployeeApiProfile Map(EmployeeJson j) => new()
    {
        Id = j.Id,
        Email = j.Email ?? "",
        FirstName = j.FirstName ?? "",
        LastName = j.LastName ?? "",
        DepartmentId = j.DepartmentId,
        ManagerId = j.ManagerId
    };

    private sealed class EmployeeJson
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
    }

    private sealed class EmployeeListJson
    {
        public Guid Id { get; set; }
    }
}
