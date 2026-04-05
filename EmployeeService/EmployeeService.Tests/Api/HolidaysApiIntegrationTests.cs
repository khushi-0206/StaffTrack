using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmployeeService.Tests.Support;

namespace EmployeeService.Tests.Api;

[TestFixture]
public class HolidaysApiIntegrationTests
{
    [Test]
    public async Task GetHolidays_AsEmployee_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");

        var response = await client.GetAsync("/api/holidays");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task PostAndDeleteHoliday_AsHr_Works()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var create = await client.PostAsJsonAsync("/api/holidays", new { name = "Labour Day", date = "2026-05-01" });
        Assert.That(create.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var id = (await create.Content.ReadFromJsonAsync<JsonElement>(TestJson.Options)).GetProperty("id").GetInt32();

        var delete = await client.DeleteAsync($"/api/holidays/{id}");
        Assert.That(delete.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}
