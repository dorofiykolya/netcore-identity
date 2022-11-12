using Microsoft.AspNetCore.Builder;

namespace Identity.Services.Identities;

public static class IdentityMiddlewareExtensions
{
    public static IApplicationBuilder UseIdentityMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<IdentityMiddleware>();
        return builder;
    }
}