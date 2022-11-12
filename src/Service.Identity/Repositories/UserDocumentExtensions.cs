using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Services.Identities;

namespace Identity.Repositories;

public static class UserDocumentExtensions
{
    public static Claim[] GenerateClaimsToRefreshToken(this UserDocument document, params Claim[] additional)
    {
        var list = new List<Claim>()
        {
            new Claim(UserClaims.TypeId, document.Id.ToString()),
            new Claim(UserClaims.TypeToken, Guid.NewGuid().ToString("N"))
        };
        list.AddRange(additional);
        return list.ToArray();
    }

    public static Claim[] ToClaims(this UserDocument document, params Claim[] additional)
    {
        var claims = new List<Claim>(document.Identities.Count + document.Roles.Count + 2 + additional.Length);
        claims.AddRange(additional);
        claims.Add(new Claim(UserClaims.TypeId, document.Id.ToString()));
        claims.Add(new Claim(UserClaims.TypeName, document.Name ?? ""));
        foreach (var identity in document.Identities)
        {
            claims.Add(new Claim($"{UserClaims.TypeIdentities}/{identity}", identity.ToString()));
        }
        foreach (var role in document.Roles)
        {
            claims.Add(new Claim(UserClaims.TypeRoles, role.Id));
        }
        return claims.ToArray();
    }

    public static async Task<UserDocument?> FindByEmailAsync(this IMongoRepository<UserDocument?> repository, string email)
    {
        var user = await repository.FindOneAsync(x =>
            x.Identities.Any(i =>
                i.Identity == Identities.Email && i.Subject == email
            )
        );
        return user;
    }

    public static async Task<UserDocument?> FindByGoogleAsync(this IMongoRepository<UserDocument?> repository, string subject)
    {
        var user = await repository.FindOneAsync(x =>
            x.Identities.Any(i =>
                i.Identity == Identities.Google && i.Subject == subject
            )
        );
        return user;
    }

    public static async Task<UserDocument?> FindByGuestAsync(this IMongoRepository<UserDocument?> repository, string subject)
    {
        var user = await repository.FindOneAsync(x =>
            x.Identities.Any(i =>
                i.Identity == Identities.Guest && i.Subject == subject
            )
        );
        return user;
    }
}