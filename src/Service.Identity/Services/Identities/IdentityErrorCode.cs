using System.ComponentModel;

namespace Identity.Services.Identities;

public enum IdentityErrorCode
{
    [Description("Ok")] Ok = 0,
    [Description("You must confirm email")] EmailNotConfirmed = 1,
    [Description("The email exists")] EmailAlreadyExist = 2,
    [Description("The email has invalid format")] EmailInvalidFormat = 3,
    [Description("The email not found")] EmailNotFound = 4,
    [Description("The email already confirmed")] EmailAlreadyConfirmed = 5,
    [Description("Invalid code")] EmailInvalidCode = 6,
    [Description("Invalid code, please register again")] EmailInvalidCodeRegisterAgain = 7,
    [Description("Invalid password")] EmailInvalidPassword = 8,
    [Description("User not found")] UserNotFound = 9,
    [Description("Unauthorized")] Unauthorized = 10,
    [Description("Google JWT error")] GoogleJwtError = 11,
    [Description("Identity not found")] IdentityNotFound = 12,
    [Description("Incorect old password")] IncorrectOldPassword = 13,
    [Description("Incorect password")] IncorrectPassword = 14,
    [Description("You are restoring password, check your email")] EmailNeedRestorePasswordState = 15,
    [Description("Many invalid password, please try later! In {0}")] EmailManyInvalidPassword = 16,
}