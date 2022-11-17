using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignOutRequest : IIdentityRequest<SignOutResponse>
    {
        public const string Route = "sign/out";

    }
}