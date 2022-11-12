using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignUpByEmailRequest : IIdentityRequest<SignUpByEmailResponse>
    {
        public const string Route = "/sign/up/email";

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
