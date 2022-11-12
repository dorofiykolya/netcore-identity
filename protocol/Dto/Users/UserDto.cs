using System;

namespace Identity.Protocol.Dto.Users
{
    [Serializable]
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IdentityDto[] Identities { get; set; }
        public RoleDto[] Roles { get; set; }
    }
}
