using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Utils;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Algorithms;

public class TokensGenerator(IConfiguration configuration)
{
    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenExpires = DateTime.UtcNow.AddMinutes(GetJwtSetting<double>("AccessTokenExpiresInMinutes"));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtSetting<string>("Key")));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: tokenExpires,
            issuer: GetJwtSetting<string>("Issuer"),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        return new RefreshToken()
        {
            Token = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddMinutes(GetJwtSetting<double>("RefreshTokenExpiresInMinutes")),
            User = user
        };
    }

    private T GetJwtSetting<T>(string key)
    {
        return configuration.GetValue<T>($"JwtSettings:{key}") ?? throw new ArgumentNullException(nameof(configuration), ErrorMessages.JwtSettingsNotFoundError);
    }
}