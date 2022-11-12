namespace Common.Jwt;

public class JwtOptions
{
    public string SecurityKey { get; set; } = "securityKey";
    public string Issuer { get; set; } = "issuer";
    public string Audience { get; set; } = "audience";
    public TimeSpan Expire { get; set; } = TimeSpan.FromMinutes(1);
}