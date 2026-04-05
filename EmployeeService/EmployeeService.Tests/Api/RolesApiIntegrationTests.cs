using System.Net;
using System.Net.Http.Json;
using EmployeeService.Application.DTOs.Roles;
using EmployeeService.Tests.Support;

namespace EmployeeService.Tests.Api;

[TestFixture]
public class RolesApiIntegrationTests
{
    [Test]
    public async Task GetRoles_AfterSeed_ReturnsDefaultRoles()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");
        await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var response = await client.GetAsync("/api/roles");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var list = await response.Content.ReadFromJsonAsync<List<RoleResponseDto>>(TestJson.Options);
        Assert.That(list, Is.Not.Null);
        Assert.That(list!.Count, Is.GreaterThanOrEqualTo(4));
        Assert.That(list.Any(r => r.Name == "HR"), Is.True);
    }

    [Test]
    public async Task PostRole_AsHr_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var response = await client.PostAsJsonAsync("/api/roles", new { name = "Contractor" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task PostRole_DuplicateName_ReturnsConflict()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        await ApiIntegrationDb.SeedDepartmentAndRolesAsync(factory.Services);

        var first = await client.PostAsJsonAsync("/api/roles", new { name = "UniqueRoleX" });
        Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var second = await client.PostAsJsonAsync("/api/roles", new { name = "UniqueRoleX" });

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
}
