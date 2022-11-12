using System.Collections.Generic;
using Identity.Services.Identities;

namespace Identity.Repositories;

public static class UserIdentityExtensions
{
    public static UserEmailIdentity? FindEmail(this IList<UserIdentity> list, string email)
    {
        foreach (var identity in list)
        {
            if (identity.Identity == Identities.Email)
            {
                if (identity.Subject == email)
                {
                    return (UserEmailIdentity)identity;
                }
            }
        }
        return null;
    }

    public static UserEmailIdentity? FindGoogle(this IList<UserIdentity> list, string subject)
    {
        foreach (var identity in list)
        {
            if (identity.Identity == Identities.Google)
            {
                if (identity.Subject == subject)
                {
                    return (UserEmailIdentity)identity;
                }
            }
        }
        return null;
    }
}