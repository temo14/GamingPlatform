using Domain.Models;

namespace Infrastructure.Configuration;


internal class LeaderboardSettings
{
    public string MaxLeaderboardEntries { get; set; } = null!;
    public string LeaderboardDataRetentionDays { get; set; } = null!;
    public string PlayerDataRetentionDays { get; set; } = null!;
    public IEnumerable<LeaderboardPrizeConfiguration> PrizeConfiguration { get; set; }
}
