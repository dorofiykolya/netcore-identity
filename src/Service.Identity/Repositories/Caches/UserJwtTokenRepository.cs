using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Jwt;
using Common.Redis;
using Identity.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Repositories.Caches;

public interface IUserJwtTokenRepository
{
    Task<Tokens> UpdateTokenWithIdentity(UserDocument user, string? identity = null);
    Task Purge(string userId);
}

public record Tokens(string AccessToken, string RefreshToken);

public class UserJwtTokenRepository : IUserJwtTokenRepository
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly ICacheRepository<UserTokenCache> _tokenCache;
    private readonly IUserScopeProvider _userScopeProvider;

    public UserJwtTokenRepository(
        IJwtGenerator jwtGenerator,
        ICacheRepository<UserTokenCache> tokenCache,
        IUserScopeProvider userScopeProvider
    )
    {
        _jwtGenerator = jwtGenerator;
        _tokenCache = tokenCache;
        _userScopeProvider = userScopeProvider;
    }

    public async Task<Tokens> UpdateTokenWithIdentity(UserDocument user, string? identity = null)
    {
        var claims = new List<Claim>();
        if (identity != null)
        {
            claims.Add(new Claim(UserClaimTypes.Identity, identity));
        }
        if (user.Roles.Count > 0)
        {
            var hasSet = new HashSet<string>();
            foreach (var userRole in user.Roles)
            {
                foreach (string value in _userScopeProvider.GetScopeByRole(userRole))
                {
                    hasSet.Add(value);
                }
            }
            foreach (string value in hasSet)
            {
                claims.Add(new Claim(UserClaimTypes.Scope, value));
            }
        }
        var additional = claims.ToArray();
        var refreshClaims = user.GenerateClaimsToRefreshToken(additional);
        var accessToken = _jwtGenerator.GenerateToken(user.ToClaims(additional));
        var refreshToken = _jwtGenerator.GenerateToken(refreshClaims);
        await _tokenCache.UpdateAsync(new UserTokenCache
        {
            UserId = user.Id.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshClaims.Token()
        });
        await _tokenCache.SaveAsync();
        return new Tokens(accessToken, refreshToken);
    }

    public async Task Purge(string userId)
    {
        await _tokenCache.DeleteAsync(new UserTokenCache
        {
            UserId = userId
        });
    }
}

public static class UserJwtTokenRepositoryExtensions
{
    public static void AddUserJwtTokenRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserJwtTokenRepository, UserJwtTokenRepository>();
    }
}