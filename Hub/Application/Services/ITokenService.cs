using Application.Features.Player.Queries.Login;
using Domain.Models;

namespace Application.Services;

public interface ITokenService
{
    string GenerateToken(Player player);
    bool IsValidCredentials(PlayerLoginQuery query, Player player);
}