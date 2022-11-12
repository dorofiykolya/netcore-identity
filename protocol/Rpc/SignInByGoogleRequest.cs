using System;
using Identity.Protocol.Api;

#pragma warning disable CS8618

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignInByGoogleRequest : IIdentityRequest<SignInByGoogleResponse>
    {
        public const string Route = "/sign/in/google";

        public string GoogleToken { get; set; }
    }
}
