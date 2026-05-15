
namespace AeonRegistry.Middleware;

public class BlockIdentityEndpoints
{
    private readonly RequestDelegate _next;

    private static readonly string[] BlockedEndpoints = [
        "/api/auth/register",
        "/api/auth/forgotpassword",
        "/api/auth/resetpassword",
        "/api/auth/manage",
        "/api/auth/manage/info",
        "/api/auth/manage/2fa",
    ];

    public BlockIdentityEndpoints(RequestDelegate next)
    {
        _next = next;
    }

    // Middleware
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (path != null && BlockedEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Endpoint not found.");
            return;
        }
        await _next(context);
    }
}
