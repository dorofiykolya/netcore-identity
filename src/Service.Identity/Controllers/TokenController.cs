using System.Net;
using System.Threading.Tasks;
using Common.Jwt;
using Common.Mongo;
using Common.Redis;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Identity.Services.Identities;
using Identity.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
public class TokenController : ControllerBase
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IMongoRepository<UserDocument?> _userRepository;
    private readonly ICacheRepository<UserTokenCache> _tokenCache;
    private readonly IUserJwtTokenRepository _userJwt;

    public TokenController(
        IJwtGenerator jwtGenerator,
        IMongoRepository<UserDocument?> userRepository,
        ICacheRepository<UserTokenCache> tokenCache,
        IUserJwtTokenRepository userJwt
    )
    {
        _jwtGenerator = jwtGenerator;
        _userRepository = userRepository;
        _tokenCache = tokenCache;
        _userJwt = userJwt;
    }

    [Authorize]
    [HttpPost]
    [Route(TokenValidateRequest.Route)]
    [ProducesResponseType(typeof(TokenValidateResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public Task<IActionResult> Verify(TokenValidateRequest request)
    {
        return Task.FromResult<IActionResult>(Ok());
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(RefreshTokenRequest.Route)]
    [ProducesResponseType(typeof(RefreshTokenResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var token = _jwtGenerator.Parse(request.RefreshToken);
        string userId = token.Claims.Sub();
        string refreshToken = token.Claims.Token();
        string identity = token.Claims.Identity();
        var tokenCache = await _tokenCache.FirstAsync(x => x.UserId == userId);
        if (tokenCache == null)
        {
            throw IdentityErrorCode.Unauthorized.Exception();
        }
        if (tokenCache.RefreshToken != refreshToken)
        {
            throw IdentityErrorCode.Unauthorized.Exception();
        }
        var user = await _userRepository.FindByIdAsync(tokenCache.UserId);
        if (user == null)
        {
            throw IdentityErrorCode.UserNotFound.Exception();
        }
        var tokens = await _userJwt.UpdateTokenWithIdentity(user, identity);
        return Ok(new RefreshTokenResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        });
    }
}