using System;
using Identity.Protocol.Api;

#pragma warning disable CS8618

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignUpByEmailRequest : IIdentityRequest<SignUpByEmailResponse>
    {
        public const string Route = "/sign/up/email";
        
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
