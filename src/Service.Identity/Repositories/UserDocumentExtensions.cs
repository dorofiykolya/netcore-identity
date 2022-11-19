using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Mongo;
using Identity.Services.Identities;
using Identity.Services.Users;

namespace Identity.Repositories;

public static class UserDocumentExtensions
{
    public static Claim[] GenerateClaimsToRefreshToken(this UserDocument document, params Claim[] additional)
    {
        var list = new List<Claim>()
        {
            new Claim(UserClaimTypes.Subject, document.Id.ToString()),
            new Claim(UserClaimTypes.Token, Guid.NewGuid().ToString("N"))
        };
        list.AddRange(additional);
        return list.ToArray();
    }

    public static Claim[] ToClaims(this UserDocument document, params Claim[] additional)
    {
        var claims = new List<Claim>(document.Identities.Count + document.Roles.Count + 2 + additional.Length);
        claims.AddRange(additional);
        claims.Add(new Claim(UserClaimTypes.Subject, document.Id.ToString()));
        claims.Add(new Claim(UserClaimTypes.Name, document.Name ?? ""));
        foreach (var identity in document.Identities)
        {
            claims.Add(new Claim($"{UserClaimTypes.Identities}/{identity}", identity.ToString() ?? string.Empty));
        }
        foreach (var role in document.Roles)
        {
            claims.Add(new Claim(UserClaimTypes.Role, role.Id));
        }
        foreach (string scope in document.Scopes)
        {
            claims.Add(new Claim(UserClaimTypes.Scope, scope));
        }
        return claims.ToArray();
    }

    public static async Task<UserDocument?> FindByEmailAsync(this IMongoRepository<UserDocument> repository, string email)
    {
        var user = await repository.FindOneAsync(x =>
            x.Identities.Any(i =>
                i.Identity == Identities.Email && i.Subject == email
            )
        );
        return user;
    }

    public static async Task<UserDocument?> FindByGoogleAsync(this IMongoRepository<UserDocument> repository, string subject)
    {
        var user = await repository.FindOneAsync(x =>
            x.Identities.Any(i =>
                i.Identity == Identities.Google && i.Subject == subject
            )
        );
        return user;
    }

    public static async Task<UserDocument?> FindByGuestAsync(this IMongoRepository<UserDocument> repository, string subject)
    {
        var user = await repository.FindOneAsync(x =>
            x.Identities.Any(i =>
                i.Identity == Identities.Guest && i.Subject == subject
            )
        );
        return user;
    }
}