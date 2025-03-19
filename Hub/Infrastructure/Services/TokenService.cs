using Application.Features.Player.Queries.Login;
using Application.Services;
using Domain.Models;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Infrastructure.Services;

internal class TokenService(
    IOptions<JwtSettings> options) : ITokenService
{
    public string GenerateToken(Player player)
    {
        var jwtSettings = options.Value;
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationInMinutes);
        var secret = Encoding.UTF8.GetBytes(jwtSettings.Key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, player.Id.ToString()),
            new(ClaimTypes.Name, player.Username)
        };
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: jwtSettings.Issuer,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool IsValidCredentials(PlayerLoginQuery query, Player player)
    {
        if (player == null)
        {
            return false;
        }
        return player.Username == query.Username;
    }
}
