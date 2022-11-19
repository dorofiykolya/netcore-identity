using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services.Passwords;

public class PasswordValidator : IPasswordValidator
{
    private readonly PasswordOptions _options;

    /// <summary>
    /// Constructions a new instance of <see cref="PasswordValidator{TUser}"/>.
    /// </summary>
    /// <param name="options">password options</param>
    /// <param name="errors">The <see cref="IdentityErrorDescriber"/> to retrieve error text from.</param>
    public PasswordValidator(PasswordOptions options, IdentityErrorDescriber? errors = null)
    {
        _options = options;
        Describer = errors ?? new IdentityErrorDescriber();
    }

    /// <summary>
    /// Gets the <see cref="IdentityErrorDescriber"/> used to supply error text.
    /// </summary>
    /// <value>The <see cref="IdentityErrorDescriber"/> used to supply error text.</value>
    public IdentityErrorDescriber Describer { get; private set; }

    /// <summary>
    /// Validates a password as an asynchronous operation.
    /// </summary>
    /// <param name="password">The password supplied for validation</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual Task<IdentityResult> ValidateAsync(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }
        var errors = new List<IdentityError>();
        var options = _options;
        if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
        {
            errors.Add(Describer.PasswordTooShort(options.RequiredLength));
        }
        if (options.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
        {
            errors.Add(Describer.PasswordRequiresNonAlphanumeric());
        }
        if (options.RequireDigit && !password.Any(IsDigit))
        {
            errors.Add(Describer.PasswordRequiresDigit());
        }
        if (options.RequireLowercase && !password.Any(IsLower))
        {
            errors.Add(Describer.PasswordRequiresLower());
        }
        if (options.RequireUppercase && !password.Any(IsUpper))
        {
            errors.Add(Describer.PasswordRequiresUpper());
        }
        if (options.RequiredUniqueChars >= 1 && password.Distinct().Count() < options.RequiredUniqueChars)
        {
            errors.Add(Describer.PasswordRequiresUniqueChars(options.RequiredUniqueChars));
        }
        return
            Task.FromResult(errors.Count == 0
                ? IdentityResult.Success
                : IdentityResult.Failed(errors.ToArray()));
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is a digit.
    /// </summary>
    /// <param name="c">The character to check if it is a digit.</param>
    /// <returns>True if the character is a digit, otherwise false.</returns>
    public virtual bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is a lower case ASCII letter.
    /// </summary>
    /// <param name="c">The character to check if it is a lower case ASCII letter.</param>
    /// <returns>True if the character is a lower case ASCII letter, otherwise false.</returns>
    public virtual bool IsLower(char c)
    {
        return c >= 'a' && c <= 'z';
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is an upper case ASCII letter.
    /// </summary>
    /// <param name="c">The character to check if it is an upper case ASCII letter.</param>
    /// <returns>True if the character is an upper case ASCII letter, otherwise false.</returns>
    public virtual bool IsUpper(char c)
    {
        return c >= 'A' && c <= 'Z';
    }

    /// <summary>
    /// Returns a flag indicating whether the supplied character is an ASCII letter or digit.
    /// </summary>
    /// <param name="c">The character to check if it is an ASCII letter or digit.</param>
    /// <returns>True if the character is an ASCII letter or digit, otherwise false.</returns>
    public virtual bool IsLetterOrDigit(char c)
    {
        return IsUpper(c) || IsLower(c) || IsDigit(c);
    }
}