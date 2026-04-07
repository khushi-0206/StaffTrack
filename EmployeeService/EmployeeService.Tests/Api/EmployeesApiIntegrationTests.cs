using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmployeeService.Application.Common.Models;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Domain.Enums;
using EmployeeService.Tests.Support;

namespace EmployeeService.Tests.Api;

[TestFixture]
public class EmployeesApiIntegrationTests
{
    [Test]
    public async Task PostEmployees_AsHr_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        var (departmentId, roleId) = await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var body = CreateEmployeeBody("Sam", "Taylor", "sam.taylor@stafftrack.test", departmentId, roleId);

        var response = await client.PostAsJsonAsync("/api/employees", body);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task GetEmployees_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/employees");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetEmployees_AsEmployee_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");

        var response = await client.GetAsync("/api/employees");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task PostEmployees_AsEmployee_ReturnsForbidden()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");
        var (departmentId, roleId) = await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var body = CreateEmployeeBody("No", "Access", "no.access@test.local", departmentId, roleId);

        var response = await client.PostAsJsonAsync("/api/employees", body);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetEmployeeById_NotFound_Returns404()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var response = await client.GetAsync($"/api/employees/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PostEmployees_DuplicateEmail_ReturnsConflict()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        var (departmentId, roleId) = await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var body = CreateEmployeeBody("First", "User", "duplicate@stafftrack.test", departmentId, roleId);
        var first = await client.PostAsJsonAsync("/api/employees", body);
        Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var second = await client.PostAsJsonAsync("/api/employees", body);

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task PutAndDeleteEmployee_AsHr_Works()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        var (departmentId, roleId) = await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var create = CreateEmployeeBody("Edit", "Me", "edit.me@stafftrack.test", departmentId, roleId);
        var created = await client.PostAsJsonAsync("/api/employees", create);
        var createdJson = await created.Content.ReadFromJsonAsync<JsonElement>(TestJson.Options);
        var id = createdJson.GetProperty("id").GetGuid();

        var update = new
        {
            firstName = "Edited",
            lastName = "Name",
            email = "edit.me@stafftrack.test",
            phone = (string?)null,
            departmentId,
            roleId,
            managerId = (Guid?)null,
            dateOfJoining = DateTime.UtcNow.AddMonths(-6),
            status = (int)EmployeeStatus.Active
        };

        var put = await client.PutAsJsonAsync($"/api/employees/{id}", update);
        Assert.That(put.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var delete = await client.DeleteAsync($"/api/employees/{id}");
        Assert.That(delete.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var getAgain = await client.GetAsync($"/api/employees/{id}");
        Assert.That(getAgain.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetEmployees_WithPagination_ReturnsPagedShape()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var response = await client.GetAsync("/api/employees?page=1&pageSize=5");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var page = await response.Content.ReadFromJsonAsync<PagedResult<EmployeeListItemDto>>(TestJson.Options);
        Assert.That(page, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(page!.Page, Is.EqualTo(1));
            Assert.That(page.PageSize, Is.EqualTo(5));
            Assert.That(page.Items, Is.Not.Null);
        });
    }

    [Test]
    public async Task PostEmployees_InvalidEmail_ReturnsBadRequest()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        var (departmentId, roleId) = await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var body = CreateEmployeeBody("Bad", "Email", "not-valid", departmentId, roleId);

        var response = await client.PostAsJsonAsync("/api/employees", body);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    private static object CreateEmployeeBody(
        string firstName,
        string lastName,
        string email,
        int departmentId,
        int roleId) => new
    {
        firstName,
        lastName,
        email,
        phone = (string?)null,
        departmentId,
        roleId,
        managerId = (Guid?)null,
        dateOfJoining = DateTime.UtcNow.AddMonths(-3),
        status = (int)EmployeeStatus.Active
    };
}
