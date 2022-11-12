using System;

namespace Identity.Services.Identities;

public class IdentityException : Exception
{
    public IdentityErrorCode Code { get; }

    public IdentityException(IdentityErrorCode code, string message) : base(message)
    {
        Code = code;
    }
}