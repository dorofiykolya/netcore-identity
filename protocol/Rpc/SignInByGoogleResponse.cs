using System;

namespace Identity.Protocol.Rpc
{
    [Serializable]
    public class SignInByGoogleResponse
    {
        public bool IsNewUser { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
