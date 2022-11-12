using Microsoft.AspNetCore.Builder;

namespace Identity.Services.Jwt;

public static class JwtValidatorMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtValidatorMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<JwtValidatorMiddleware>();
        return builder;
    }
}