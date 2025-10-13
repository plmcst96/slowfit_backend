using Microsoft.Extensions.Options;
using slowfit.JWT;

public class JWTConfiguration
{
    private readonly RequestDelegate _next;
    private readonly IOptions<JWTModel> _jwt;

    public JWTConfiguration(RequestDelegate next, IOptions<JWTModel> jwtModel)
    {
        _next = next;
        _jwt = jwtModel;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("slowKey", out var extractedApiKey))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Endpoint access unauthorized: slowKey not found.");
            return;
        }

        var apiKey = _jwt.Value.slowKey;
        Console.WriteLine($"Expected API Key: {apiKey}, Provided API Key: {extractedApiKey}");

        if (apiKey == null || !apiKey.Equals(extractedApiKey))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Endpoint access unauthorized: invalid slowKey.");
            return;
        }

        await _next(httpContext);
    }
}
