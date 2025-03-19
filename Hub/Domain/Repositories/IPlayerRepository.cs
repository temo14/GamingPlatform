using Domain.Models;

namespace Domain.Repositories;

public interface IPlayerRepository
{
    Task AddPlayerAsync(Player player, CancellationToken cancellationToken);
    Task<Player?> GetPlayerByUsernameAsync(string userName, CancellationToken cancellationToken);
}
