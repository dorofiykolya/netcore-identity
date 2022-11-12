using System;
using Identity.Protocol.Api;

#pragma warning disable CS8618

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignInByEmailRequest : IIdentityRequest<SignInByEmailResponse>
    {
        public const string Route = "/sign/in/email";

        public string Email { get; set; }
        public string Password { get; set; }
    }
}
