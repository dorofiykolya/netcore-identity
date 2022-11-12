using System;
using Identity.Services.Identities;

namespace Identity.Repositories;

[Serializable]
public class UserGoogleIdentity : UserIdentity
{
    public override string Identity { get; set; } = Identities.Google;
    public string Email { get; set; } = null!;
}