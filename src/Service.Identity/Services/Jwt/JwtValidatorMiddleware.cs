using System;
using System.Threading.Tasks;
using Common.Jwt;
using Common.Redis;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Identity.Services.Identities;
using Identity.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Identity.Services.Jwt;

public class JwtValidatorMiddleware
{
    private readonly RequestDelegate _next;

    public JwtValidatorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context,
        IJwtGenerator jwtGenerator,
        ICacheRepository<UserTokenCache> tokenRepository
    )
    {
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            var token = GetAuthorizationToken(context.Request.Headers);
            if (token == null)
            {
                throw IdentityErrorCode.Unauthorized.Exception();
            }
            var info = jwtGenerator.Parse(token);
            var userId = info.Claims.Sub();

            var user = await tokenRepository.FindByIdAsync(userId);
            if (user == null || user.AccessToken != token)
            {
                throw IdentityErrorCode.Unauthorized.Exception();
            }
        }

        await _next(context);
    }

    private static string? GetAuthorizationToken(IHeaderDictionary headers)
    {
        if (headers.TryGetValue(HeaderNames.Authorization, out var authorization))
        {
            var token = (string)authorization;

            var index = token.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                token = token.Substring(index + 1);
            }

            return token;
        }

        return null;
    }
}