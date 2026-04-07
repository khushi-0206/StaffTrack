using System.Net;
using System.Net.Http.Json;
using EmployeeService.Application.DTOs.Departments;
using EmployeeService.Tests.Support;

namespace EmployeeService.Tests.Api;

[TestFixture]
public class DepartmentsApiIntegrationTests
{
    [Test]
    public async Task GetDepartments_AsEmployee_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");
        await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var response = await client.GetAsync("/api/departments");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var list = await response.Content.ReadFromJsonAsync<List<DepartmentResponseDto>>(TestJson.Options);
        Assert.That(list, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task PostDepartment_AsHr_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var response = await client.PostAsJsonAsync("/api/departments", new { name = "Finance", description = "Finance team" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task PostDepartment_AsEmployee_ReturnsForbidden()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");
        await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var response = await client.PostAsJsonAsync("/api/departments", new { name = "Blocked", description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetDepartmentById_AsHr_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        var (deptId, _) = await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var response = await client.GetAsync($"/api/departments/{deptId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var dto = await response.Content.ReadFromJsonAsync<DepartmentResponseDto>(TestJson.Options);
        Assert.That(dto?.Name, Is.EqualTo("Engineering"));
    }
}
