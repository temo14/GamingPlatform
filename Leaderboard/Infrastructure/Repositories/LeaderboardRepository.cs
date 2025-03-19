using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

internal class LeaderboardRepository(
    LeaderboardDbContext dbContext) : ILeaderboardRepository
{
    public async Task AddLeaderboardEntryAsync(LeaderboardEntry entry)
    {
        await dbContext.LeaderboardEntries.AddAsync(entry);
        await dbContext.SaveChangesAsync();
    }
}