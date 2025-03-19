using Domain.Models;
using Domain.Repositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

internal class BetRepository(
    ApplicationDbContext dbContext) : IBetRepository
{
    public async Task SaveBetAsync(Bet betEvent, CancellationToken cancellationToken)
    {
        await dbContext.Bets.AddAsync(betEvent, cancellationToken);
        dbContext.SaveChanges();
    }
}