using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Identity.Repositories;

public static class UserClaimsExtensions
{
    public static string Id(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaims.TypeId).Value;
    public static string Token(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaims.TypeToken).Value;
    public static string Identity(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaims.TypeIdentity).Value;
    public static Claim IdentityClaim(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaims.TypeIdentity);
    public static IEnumerable<Claim> Roles(this IEnumerable<Claim> claims) => claims.Where(c => c.Type == UserClaims.TypeRoles);
    public static bool IsAdmin(this IEnumerable<Claim> claims) => claims.Roles().Any(x => x.Value == UserRoles.Admin);
}