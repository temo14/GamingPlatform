using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

internal class PrizeRepository(
    LeaderboardDbContext context) : IPrizeRepository
{
    public async Task AddPrizeAsync(Prize prize)
    {
        await context.Prizes.AddAsync(prize);
        await context.SaveChangesAsync();
    }
}
