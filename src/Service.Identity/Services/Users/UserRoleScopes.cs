using System;

namespace Identity.Services.Users;

[Serializable]
public class UserRoleScopes
{
    public string Role { get; set; } = null!;
    public string[]? Scope { get; set; }
}