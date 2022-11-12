using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Identity.Services.Identities;

public class IdentityMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<IdentityMiddleware> _logger;
    public IdentityMiddleware(RequestDelegate next, ILogger<IdentityMiddleware> logger)
    {
        _next = next;

        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (IdentityException ex)
        {
            _logger.LogError(ex, "Failed to handle request {RequestPath}. Request validation error {ErrorCode}", context.Request.Path, ex.Message);

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync($"{{\"code\":{(int)ex.Code}, \"message\": \"{ex.Message}\"}}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle request {RequestPath}. Internal server error {ErrorMessage}", context.Request.Path, ex.Message);

            context.Response.Clear();

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsync(string.Empty);
        }
    }
}