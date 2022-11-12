using System;

namespace Identity.Protocol.Dto.Users
{
    [Serializable]
    public class IdentityDto
    {
        public string Identity { get; set; }
        public string Subject { get; set; }
    }
}
