using System.Net;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Protocol.Rpc;
using Identity.Repositories;
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
    private readonly IPasswordValidator _passwordValidator;
    private readonly IEmailSender _emailSender;
    private readonly IPasswordGenerator _passwordGenerator;

    public UserEmailController(
        IMongoRepository<UserDocument> userRepository,
        IPasswordValidator passwordValidator,
        IEmailSender emailSender,
        IPasswordGenerator passwordGenerator
    )
    {
        _userRepository = userRepository;
        _passwordValidator = passwordValidator;
        _emailSender = emailSender;
        _passwordGenerator = passwordGenerator;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(ForgotEmailPasswordRequest.Route)]
    [ProducesResponseType(typeof(ForgotEmailPasswordResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ForgotPassword(ForgotEmailPasswordRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw IdentityErrorCode.EmailNotFound.Exception();
        }
        string newPassword = await _passwordGenerator.GenerateAsync();
        var email = user.Identities.FindEmail(request.Email)!;
        if (!email.Confirmed)
        {
            throw IdentityErrorCode.EmailNotConfirmed.Exception();
        }
        email.Password = newPassword;
        await _userRepository.ReplaceOneAsync(user);
        await _emailSender.SendAsync($"New password: {newPassword}", request.Email, "Forgot email");
        return Ok(new ForgotEmailPasswordResponse());
    }

    [Authorize]
    [HttpPost]
    [Route(ChangeEmailPasswordRequest.Route)]
    [ProducesResponseType(typeof(ChangeEmailPasswordResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangeEmailPasswordRequest request)
    {
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