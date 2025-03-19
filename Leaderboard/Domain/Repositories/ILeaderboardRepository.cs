using Domain.Entities;

namespace Domain.Repositories;

public interface ILeaderboardRepository
{
    Task AddLeaderboardEntryAsync(LeaderboardEntry entry);
}
