using System;
using Identity.Protocol.Api;

#pragma warning disable CS8618

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class RefreshTokenRequest : IIdentityRequest<RefreshTokenResponse>
    {
        public const string Route = "/token/refresh";

        public string RefreshToken { get; set; }
    }
}
