using Application.Features.Compute.CurrentLeaderboard.DTOs;
using Application.Services;
using Domain.Entities;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

internal class LeaderboardAggregator(
        IConnectionMultiplexer redis,
        IOptions<LeaderboardSettings> settings,
        ILogger<LeaderboardAggregator> logger) : ILeaderboardAggregator
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly LeaderboardSettings _settings = settings.Value;

    public async Task AddBetAsync(Bet betEvent)
    {
        var hourKey = GetHourKey(betEvent.CreatedAt);
        var playerKey = $"player:{betEvent.PlayerId}";

        await _db.SortedSetIncrementAsync(hourKey, betEvent.PlayerId.ToString(), (double)betEvent.Amount);
        await _db.HashSetAsync(playerKey, "PlayerName", betEvent.PlayerName);
        await _db.KeyExpireAsync(hourKey, TimeSpan.FromDays(int.Parse(_settings.LeaderboardDataRetentionDays)));
        await _db.KeyExpireAsync(playerKey, TimeSpan.FromDays(int.Parse(_settings.PlayerDataRetentionDays)));

        logger.LogDebug("Bet added for Player {PlayerId}, Amount: {Amount}, HourKey: {HourKey}",
            betEvent.PlayerId, betEvent.Amount, hourKey);
    }

    public async Task<List<LeaderboardEntryDto>> GetCurrentHourLeaderboardAsync()
    {
        return await GetHourLeaderboardAsync(DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour));
    }

    public async Task<List<LeaderboardEntryDto>> GetHourLeaderboardAsync(DateTime hour)
    {
        var hourKey = GetHourKey(hour);
        var entries = await _db.SortedSetRangeByRankWithScoresAsync(
            hourKey, 0, int.Parse(_settings.MaxLeaderboardEntries) - 1, Order.Descending);

        if (entries.Length == 0) return [];

        var result = new List<LeaderboardEntryDto>();
        int rank = 1;

        foreach (var entry in entries)
        {
            var playerId = Guid.Parse(entry.Element);
            var playerName = (await _db.HashGetAsync($"player:{playerId}", "PlayerName")).ToString();

            result.Add(new LeaderboardEntryDto
            {
                PlayerId = playerId,
                PlayerName = string.IsNullOrEmpty(playerName) ? "Unknown Player" : playerName,
                TotalBets = (decimal)entry.Score,
                Rank = rank++
            });
        }

        return result;
    }

    private string GetHourKey(DateTime dateTime)
    {
        var utcDateTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();

        var truncatedDateTime = new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, utcDateTime.Hour, 0, 0, DateTimeKind.Utc);

        return $"leaderboard:{truncatedDateTime:yyyy-MM-dd-HH}";
    }
}
