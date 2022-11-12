using System;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignInByGuestResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
