using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignInByGuestRequest : IIdentityRequest<SignInByGuestResponse>
    {
        public const string Route = "/sign/in/guest";

        public string Id { get; set; }
    }
}
