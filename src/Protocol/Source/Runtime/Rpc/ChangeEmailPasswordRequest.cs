using System;
using Identity.Protocol.Api;

#pragma warning disable CS8618

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class ChangeEmailPasswordRequest : IIdentityRequest<ChangeEmailPasswordResponse>
    {
        public const string Route = "user/email/change";

        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}