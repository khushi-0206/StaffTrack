using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmployeeService.Application.DTOs.LeaveTypes;
using EmployeeService.Tests.Support;

namespace EmployeeService.Tests.Api;

[TestFixture]
public class LeaveTypesApiIntegrationTests
{
    [Test]
    public async Task GetLeaveTypes_AsEmployee_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");

        var response = await client.GetAsync("/api/leavetypes");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var list = await response.Content.ReadFromJsonAsync<List<LeaveTypeResponseDto>>(TestJson.Options);
        Assert.That(list, Is.Not.Null);
    }

    [Test]
    public async Task PostLeaveType_AsHr_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var response = await client.PostAsJsonAsync("/api/leavetypes", new { name = "Sick", maxDays = 10 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task PostLeaveType_AsEmployee_ReturnsForbidden()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");

        var response = await client.PostAsJsonAsync("/api/leavetypes", new { name = "Illegal", maxDays = 5 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task PutLeaveType_AsHr_ReturnsNoContent()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var create = await client.PostAsJsonAsync("/api/leavetypes", new { name = "Parental", maxDays = 30 });
        Assert.That(create.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var id = (await create.Content.ReadFromJsonAsync<JsonElement>(TestJson.Options)).GetProperty("id").GetInt32();

        var put = await client.PutAsJsonAsync($"/api/leavetypes/{id}", new { name = "Parental updated", maxDays = 40 });

        Assert.That(put.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}
