using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignUpEmailConfirmRequest : IIdentityRequest<SignUpEmailConfirmResponse>
    {
        public const string Route = "/sign/up/email/confirm";

        public string Email { get; set; }
        public string Code { get; set; }
    }
}
