using System.Net;
using System.Net.Http.Json;
using LeaveService.Tests.Support;

namespace LeaveService.Tests.Api;

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
    }

    [Test]
    public async Task PostLeaveType_AsHr_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var response = await client.PostAsJsonAsync("/api/leavetypes", new { name = "Bereavement", maxDays = 5 });

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
}
