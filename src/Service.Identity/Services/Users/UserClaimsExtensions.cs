using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Identity.Services.Users;

public static class UserClaimsExtensions
{
    public static string Sub(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaimTypes.Subject).Value;
    public static string Token(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaimTypes.Token).Value;
    public static string Identity(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaimTypes.Identity).Value;
    public static Claim IdentityClaim(this IEnumerable<Claim> claims) => claims.First(x => x.Type == UserClaimTypes.Identity);
    public static IEnumerable<Claim> Roles(this IEnumerable<Claim> claims) => claims.Where(c => c.Type == UserClaimTypes.Role);
    public static bool IsAdmin(this IEnumerable<Claim> claims) => claims.Roles().Any(x => x.Value == UserRoles.Admin);
    public static bool IsUser(this IEnumerable<Claim> claims) => claims.Roles().Any(x => x.Value == UserRoles.User);
    public static bool IsGuest(this IEnumerable<Claim> claims) => claims.Roles().Any(x => x.Value == UserRoles.Guest);
    public static IEnumerable<Claim> Scopes(this IEnumerable<Claim> claims) => claims.Where(x => x.Type == UserClaimTypes.Scope);
}