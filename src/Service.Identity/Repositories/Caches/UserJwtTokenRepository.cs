using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Jwt;
using Common.Redis;
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

    public UserJwtTokenRepository(
        IJwtGenerator jwtGenerator,
        ICacheRepository<UserTokenCache> tokenCache
    )
    {
        _jwtGenerator = jwtGenerator;
        _tokenCache = tokenCache;
    }

    public async Task<Tokens> UpdateTokenWithIdentity(UserDocument user, string? identity = null)
    {
        var additional = identity != null ?
            new[]
            {
                new Claim(UserClaims.TypeIdentity, identity)
            }
            : Array.Empty<Claim>();
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