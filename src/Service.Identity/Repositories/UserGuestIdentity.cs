using Identity.Services.Identities;

namespace Identity.Repositories;

public class UserGuestIdentity : UserIdentity
{
    public override string Identity { get; set; } = Identities.Guest;
}