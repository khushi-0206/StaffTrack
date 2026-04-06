using System.Net;
using System.Net.Http.Json;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Tests.Support;

namespace LeaveService.Tests.Api;

[TestFixture]
public class LeavesApiIntegrationTests
{
    [Test]
    public async Task PostLeave_AsEmployee_ReturnsCreated()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");

        var body = new ApplyLeaveRequestDto(
            StubEmployeeServiceClient.TestEmployeeId,
            1,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 3),
            "Family event");

        var response = await client.PostAsJsonAsync("/api/leaves", body);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task GetLeaves_AsHr_ReturnsOk()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("HR");

        var response = await client.GetAsync("/api/leaves");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetLeaves_AsEmployee_ReturnsForbidden()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient("Employee");

        var response = await client.GetAsync("/api/leaves");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}
