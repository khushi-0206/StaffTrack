using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveService.Tests.Support;

internal static class ApiTestClientFactory
{
    public static HttpClient CreateAuthenticatedClient(
        this TestWebApplicationFactory factory,
        string role)
    {
        var client = factory.CreateClient();
        var config = factory.Services.GetRequiredService<IConfiguration>();
        var token = JwtTokenBuilder.CreateToken(config, role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
