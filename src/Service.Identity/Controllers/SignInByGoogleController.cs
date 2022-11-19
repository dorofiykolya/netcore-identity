using System.Net;
using System.Threading.Tasks;
using Common.Mongo;
using Google.Apis.Auth;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Identity.Services.Identities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.Controllers;

[ApiController]
public class SignInByGoogleController : ControllerBase
{
    private readonly IMongoRepository<UserDocument> _userRepository;
    private readonly ILogger<SignInByGoogleController> _logger;
    private readonly IUserJwtTokenRepository _userJwt;

    public SignInByGoogleController(
        IMongoRepository<UserDocument> userRepository,
        ILogger<SignInByGoogleController> logger,
        IUserJwtTokenRepository userJwt
    )
    {
        _userRepository = userRepository;
        _logger = logger;
        _userJwt = userJwt;
    }

    [HttpPost]
    [Route(SignInByGoogleRequest.Route)]
    [ProducesResponseType(typeof(SignInByGoogleResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignInByGoogle(SignInByGoogleRequest request)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                jwt: request.GoogleToken,
                clock: null,
                forceGoogleCertRefresh: false
            );
            if (payload != null && !string.IsNullOrEmpty(payload.Subject))
            {
                bool isNewUser = false;
                var user = await _userRepository.FindByGoogleAsync(payload.Subject);
                if (user == null)
                {
                    isNewUser = true;

                    user = await _userRepository.CreateUser(document =>
                    {
                        document.Name = payload.Name ?? payload.GivenName ?? payload.Email ?? "google";
                        document.Identities.Add(new UserGoogleIdentity
                        {
                            Subject = payload.Subject,
                            Email = payload.Email,
                        });
                    });
                }

                var tokens = await _userJwt.UpdateTokenWithIdentity(user, Identities.Google);

                return Ok(new SignInByGoogleResponse
                {
                    IsNewUser = isNewUser,
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                });
            }
        }
        catch (InvalidJwtException exception)
        {
            _logger.LogError(exception, "google payload exception");
            throw IdentityErrorCode.GoogleJwtError.Exception();
        }
        return BadRequest();
    }
}