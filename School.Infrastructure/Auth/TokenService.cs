using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using School.Application.Interfaces.Auth;
using School.Domain.Entities;

namespace School.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(string key, string issuer, string audience)
    {
        _key = key;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };

        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}