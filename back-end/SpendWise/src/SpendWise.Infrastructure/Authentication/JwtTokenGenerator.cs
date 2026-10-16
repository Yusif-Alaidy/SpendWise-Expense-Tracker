// SpendWise.Infrastructure/Authentication/JwtTokenGenerator.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpendWise.Application.Common.Interfaces;

namespace SpendWise.Infrastructure.Authentication;

public class JwtTokenGenerator(IOptions<JwtSettings> options, TimeProvider clock) : IJwtTokenGenerator
{
    private readonly JwtSettings _settings = options.Value;

    public (string Token, DateTime ExpiresAt) Generate(Guid userId, string email, string displayName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, displayName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = clock.GetUtcNow().UtcDateTime.AddMinutes(_settings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}