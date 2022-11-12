using System;
using Identity.Protocol.Api;

#pragma warning disable CS8618

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class ForgotEmailPasswordRequest : IIdentityRequest<ForgotEmailPasswordResponse>
    {
        public const string Route = "user/email/forgot";

        public string Email { get; set; }
    }
}
