using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class TokenValidateRequest : IIdentityRequest<TokenValidateResponse>
    {
        public const string Route = "token/validate";
    }

}
