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
public class UserEmailController : ControllerBase
{
    private readonly IMongoRepository<UserDocument> _userRepository;
    private readonly ICacheRepository<UserEmailBlockCache> _userEmailBlock;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _emailOptions;
    private readonly InvalidPasswordOptions _invalidPasswordOptions;

    public UserEmailController(
        IMongoRepository<UserDocument> userRepository,
        ICacheRepository<UserEmailBlockCache> userEmailBlock,
        IPasswordValidator passwordValidator,
        IEmailSender emailSender,
        EmailOptions emailOptions,
        InvalidPasswordOptions invalidPasswordOptions
    )
    {
        _userRepository = userRepository;
        _userEmailBlock = userEmailBlock;
        _passwordValidator = passwordValidator;
        _emailSender = emailSender;
        _emailOptions = emailOptions;
        _invalidPasswordOptions = invalidPasswordOptions;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(ForgotEmailPasswordRequest.Route)]
    [ProducesResponseType(typeof(ForgotEmailPasswordResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ForgotPassword(ForgotEmailPasswordRequest request)
    {
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
            throw IdentityErrorCode.EmailNotFound.Exception();
        }

        var email = user.Identities.FindEmail(request.Email)!;
        if (!email.Confirmed)
        {
            throw IdentityErrorCode.EmailNotConfirmed.Exception();
        }

        await _emailSender.SendAsync($"You are trying to restore the password!<br>Your password: <b> {email.Password} </b>, please delete this message. If you do not expect this message, change your password please! For security reason!", request.Email, $"{_emailOptions.From}! Restore password");
        return Ok(new ForgotEmailPasswordResponse());
    }

    [Authorize]
    [HttpPost]
    [Route(ChangeEmailPasswordRequest.Route)]
    [ProducesResponseType(typeof(ChangeEmailPasswordResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangeEmailPasswordRequest request)
    {
        var emailCache = await _userEmailBlock.FindByIdAsync(request.Email);
        if (emailCache != null)
        {
            if (emailCache.InvalidPasswordCount > _invalidPasswordOptions.InvalidCountToBlock)
            {
                throw IdentityErrorCode.EmailManyInvalidPassword.Exception(_invalidPasswordOptions.ToBlockTimeHumanReadable());
            }
        }
        var user = await _userRepository.FindByIdAsync(User.Claims.Id());
        if (user == null)
        {
            throw IdentityErrorCode.UserNotFound.Exception();
        }
        var emailIdentity = user.Identities.FindEmail(request.Email);
        if (emailIdentity == null)
        {
            throw IdentityErrorCode.IdentityNotFound.Exception();
        }
        if (emailIdentity.Password != request.OldPassword)
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
            throw IdentityErrorCode.IncorrectOldPassword.Exception();
        }
        var passwordState = await _passwordValidator.ValidateAsync(request.NewPassword);
        if (passwordState.Succeeded)
        {
            emailIdentity.Password = request.NewPassword;
            await _userRepository.ReplaceOneAsync(user);
            return Ok(new ChangeEmailPasswordResponse());
        }

        throw IdentityErrorCode.IncorrectPassword.Exception();
    }
}