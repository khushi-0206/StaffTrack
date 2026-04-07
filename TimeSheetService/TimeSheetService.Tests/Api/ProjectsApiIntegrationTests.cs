using System.Net;
using System.Net.Http.Json;
using TimeSheetService.Tests.Support;

namespace TimeSheetService.Tests.Api;

[TestFixture]
public class ProjectsApiIntegrationTests
{
    [Test]
    public async Task GetProjects_AsEmployee_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");
        var response = await client.GetAsync("/api/projects");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task PostProject_AsHr_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");
        var response = await client.PostAsJsonAsync("/api/projects", new { name = "Alpha", description = "Test" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
}
