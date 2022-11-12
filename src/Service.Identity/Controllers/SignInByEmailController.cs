using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Identity.Services.Emails;
using Identity.Services.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
public class SignInByEmailController : ControllerBase
{
    private readonly IMongoRepository<UserDocument?> _userRepository;
    private readonly IUserJwtTokenRepository _userJwt;
    private readonly IEmailValidator _emailValidator;

    public SignInByEmailController(
        IMongoRepository<UserDocument?> userRepository,
        IUserJwtTokenRepository userJwt,
        IEmailValidator emailValidator
    )
    {
        _userRepository = userRepository;
        _userJwt = userJwt;
        _emailValidator = emailValidator;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(SignInByEmailRequest.Route)]
    [ProducesResponseType(typeof(SignInByEmailResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignInByEmail(SignInByEmailRequest request)
    {
        bool isEmailValid = await _emailValidator.ValidateAsync(request.Email);
        if (!isEmailValid)
        {
            throw IdentityErrorCode.EmailInvalidFormat.Exception();
        }

        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw IdentityErrorCode.UserNotFound.Exception();
        }

        var email = user.Identities.FindEmail(request.Email)!;
        if (!email.Confirmed)
        {
            throw IdentityErrorCode.EmailNotConfirmed.Exception();
        }

        if (email?.Password != request.Password)
        {
            throw IdentityErrorCode.IncorrectPassword.Exception();
        }

        var tokens = await _userJwt.UpdateTokenWithIdentity(user, Identities.Email);

        return Ok(new SignInByEmailResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        });
    }
}