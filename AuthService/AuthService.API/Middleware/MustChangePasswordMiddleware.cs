namespace AuthService.API.Middleware;

/// <summary>
/// Blocks authenticated requests when the JWT carries must_change_password until POST /api/auth/change-password succeeds.
/// </summary>
public class MustChangePasswordMiddleware
{
    private readonly RequestDelegate _next;

    public MustChangePasswordMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var mustChange = context.User.FindFirst("must_change_password")?.Value == "true";
            if (mustChange)
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (!path.StartsWith("/api/auth/change-password", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "You must change your password before using this service."
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}
