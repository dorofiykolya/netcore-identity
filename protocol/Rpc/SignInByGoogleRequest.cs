using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignInByGoogleRequest : IIdentityRequest<SignInByGoogleResponse>
    {
        public const string Route = "/sign/in/google";

        public string GoogleToken { get; set; }
    }

}
