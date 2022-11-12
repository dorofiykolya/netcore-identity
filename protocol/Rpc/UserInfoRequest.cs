using System;
using Identity.Protocol.Api;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class UserInfoRequest : IIdentityRequest<UserInfoResponse>
    {
        public const string Route = "/user/info";
    }
}