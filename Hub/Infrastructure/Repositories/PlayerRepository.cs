using Domain.Models;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class PlayerRepository(
    ApplicationDbContext dbContext) : IPlayerRepository
{
    public async Task AddPlayerAsync(Player player, CancellationToken cancellationToken)
    {
        await dbContext.Players.AddAsync(player, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Player?> GetPlayerByUsernameAsync(string userName, CancellationToken cancellationToken)
    {
        return await dbContext.Players.FirstOrDefaultAsync(dbContext => dbContext.Username == userName, cancellationToken);
    }
}
