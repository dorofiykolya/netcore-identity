using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class RefreshTokenRequest : IIdentityRequest<RefreshTokenResponse>
    {
        public const string Route = "/token/refresh";

        public string RefreshToken { get; set; }
    }
}
