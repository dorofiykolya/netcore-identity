using System;
using System.Net;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Protocol.Rpc;
using Identity.Repositories;
using Identity.Services;
using Identity.Services.Emails;
using Identity.Services.Identities;
using Identity.Services.Passwords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[ApiController]
public class SignUpByEmailController : ControllerBase
{
    private readonly IMongoRepository<UserDocument?> _userRepository;
    private readonly IEmailValidator _emailValidator;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IEmailSender _emailSender;

    public SignUpByEmailController(
        IMongoRepository<UserDocument?> userRepository,
        IEmailSender emailSender,
        IEmailValidator emailValidator,
        IPasswordValidator passwordValidator
    )
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _emailValidator = emailValidator;
        _passwordValidator = passwordValidator;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(SignUpByEmailRequest.Route)]
    [ProducesResponseType(typeof(SignUpByEmailResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignUpByEmail(SignUpByEmailRequest request)
    {
        bool isEmailValid = await _emailValidator.ValidateAsync(request.Email);
        if (!isEmailValid)
        {
            throw IdentityErrorCode.EmailInvalidFormat.Exception();
        }
        var isPasswordValid = await _passwordValidator.ValidateAsync(request.Password);
        if (!isPasswordValid.Succeeded)
        {
            throw IdentityErrorCode.EmailInvalidPassword.Exception();
        }

        var user = await _userRepository.FindByEmailAsync(request.Email);

        string validationCode = new Random().Next(1000, 9999).ToString();

        if (user == null)
        {
            await _emailSender.SendAsync(validationCode, request.Email, "email confirmation");
            user = await _userRepository.CreateUser();

            user.Identities.Add(new UserEmailIdentity
            {
                Email = request.Email,
                Name = request.Name,
                Password = request.Password,
                Confirmed = false,
                ValidateCode = validationCode,
                InvalidCodeCount = 0
            });

            await _userRepository.ReplaceOneAsync(user);
        }
        else
        {
            var email = user.Identities.FindEmail(request.Email);
            if (email == null)
            {
                await _emailSender.SendAsync(validationCode, request.Email, "email confirmation");
                user.Identities.Add(new UserEmailIdentity
                {
                    Email = request.Email,
                    Name = request.Name,
                    Password = request.Password,
                    Confirmed = false,
                    ValidateCode = validationCode,
                    InvalidCodeCount = 0
                });
                await _userRepository.ReplaceOneAsync(user);
            }
            else if (email.Confirmed)
            {
                throw IdentityErrorCode.EmailAlreadyExist.Exception();
            }
            else if (!email.Confirmed)
            {
                await _emailSender.SendAsync(validationCode, email.Email, "email confirmation");
                email.ValidateCode = validationCode;
                await _userRepository.ReplaceOneAsync(user);
            }
        }
        return Ok(new SignUpByEmailResponse());
    }

    [AllowAnonymous]
    [HttpPost]
    [Route(SignUpEmailConfirmRequest.Route)]
    [ProducesResponseType(typeof(SignUpEmailConfirmResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignUpByEmailConfirm(SignUpEmailConfirmRequest request)
    {
        bool isEmailValid = await _emailValidator.ValidateAsync(request.Email);
        if (!isEmailValid)
        {
            throw IdentityErrorCode.EmailInvalidFormat.Exception();
        }

        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw IdentityErrorCode.EmailNotFound.Exception();
        }
        var email = user.Identities.FindEmail(request.Email)!;
        if (email.Confirmed)
        {
            throw IdentityErrorCode.EmailAlreadyConfirmed.Exception();
        }
        if (email.ValidateCode != request.Code)
        {
            email.InvalidCodeCount++;
            if (email.InvalidCodeCount >= 5)
            {
                throw IdentityErrorCode.EmailInvalidCodeRegisterAgain.Exception();
            }
            
            await _userRepository.ReplaceOneAsync(user);

            throw IdentityErrorCode.EmailInvalidCode.Exception();
        }

        email.Confirmed = true;
        await _userRepository.ReplaceOneAsync(user);

        return Ok(new SignUpEmailConfirmResponse());
    }
}