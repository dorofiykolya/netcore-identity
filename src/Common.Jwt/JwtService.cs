using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Common.Jwt;

public interface IJwtGenerator
{
    string GenerateToken(Claim[] claims);
    JwtSecurityToken Parse(string token);
}

public class JwtGenerator : IJwtGenerator
{
    private readonly JwtOptions _options;

    public JwtGenerator(JwtOptions options)
    {
        _options = options;
    }

    public string GenerateToken(Claim[] claims)
    {
        return Generate(claims, _options.SecurityKey);
    }

    public JwtSecurityToken Parse(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var securityToken = handler.ReadJwtToken(token);

        return securityToken;
    }

    private string Generate(Claim[] claims, string secret)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var securityToken = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.Add(_options.Expire),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}