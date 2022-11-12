using System;
using Identity.Protocol.Dto.Users;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class UserInfoResponse
    {
        public UserDto User { get; set; } = null!;
    }
}
