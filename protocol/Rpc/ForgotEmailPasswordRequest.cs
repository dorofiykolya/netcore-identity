using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class ForgotEmailPasswordRequest : IIdentityRequest<ForgotEmailPasswordResponse>
    {
        public const string Route = "user/email/forgot";

        public string Email { get; set; }
    }

}
