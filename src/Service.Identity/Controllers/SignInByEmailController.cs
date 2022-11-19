using System.Net;
using System.Threading.Tasks;
using Common.Mongo;
using Common.Redis;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Repositories.Caches;
using Identity.Services.Emails;
using Identity.Services.Identities;
using Identity.Services.Passwords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
public class SignInByEmailController : ControllerBase
{
    private readonly IMongoRepository<UserDocument> _userRepository;
    private readonly ICacheRepository<UserEmailBlockCache> _userEmailBlock;
    private readonly IUserJwtTokenRepository _userJwt;
    private readonly IEmailValidator _emailValidator;
    private readonly InvalidPasswordOptions _invalidPasswordOptions;

    public SignInByEmailController(
        IMongoRepository<UserDocument> userRepository,
        ICacheRepository<UserEmailBlockCache> userEmailBlock,
        IUserJwtTokenRepository userJwt,
        IEmailValidator emailValidator,
        InvalidPasswordOptions invalidPasswordOptions
    )
    {
        _userRepository = userRepository;
        _userEmailBlock = userEmailBlock;
        _userJwt = userJwt;
        _emailValidator = emailValidator;
        _invalidPasswordOptions = invalidPasswordOptions;
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

        var emailCache = await _userEmailBlock.FindByIdAsync(request.Email);
        if (emailCache != null)
        {
            if (emailCache.InvalidPasswordCount > _invalidPasswordOptions.InvalidCountToBlock)
            {
                throw IdentityErrorCode.EmailManyInvalidPassword.Exception(_invalidPasswordOptions.ToBlockTimeHumanReadable());
            }
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

        if (email.Password != request.Password)
        {
            if (emailCache != null)
            {
                emailCache.InvalidPasswordCount++;
                await _userEmailBlock.UpdateAsync(emailCache);
                await _userEmailBlock.SaveAsync();
            }
            else
            {
                await _userEmailBlock.InsertAsync(new UserEmailBlockCache
                {
                    Email = request.Email, InvalidPasswordCount = 1
                }, _invalidPasswordOptions.BlockTime);
            }

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