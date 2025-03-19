using Application.Features.Compute.CurrentLeaderboard.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

internal class LeaderboardService(
    ILeaderboardAggregator leaderboardAggregator,
    ILeaderboardRepository leaderboardRepository,
    IPrizeRepository prizeRepository,
    IMessagePublisher messagePublisher,
    IOptions<LeaderboardSettings> settings,
    ILogger<LeaderboardService> logger) : ILeaderboardService
{
    private readonly LeaderboardSettings _settings = settings.Value;

    public async Task ProcessBetAsync(Bet betEvent)
    {
        await leaderboardAggregator.AddBetAsync(betEvent);
    }

    public async Task<List<LeaderboardEntryDto>> GetCurrentLeaderboardAsync()
    {
        return await leaderboardAggregator.GetCurrentHourLeaderboardAsync();
    }

    public async Task ProcessHourlyLeaderboardAsync()
    {
        var previousHour = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            DateTime.UtcNow.Day,
            DateTime.UtcNow.Hour,
            0, 0, DateTimeKind.Utc);

        logger.LogInformation($"Processing leaderboard for hour {previousHour}");

        var leaderboard = await leaderboardAggregator.GetHourLeaderboardAsync(previousHour);
        if (leaderboard.Count == 0)
        {
            logger.LogWarning("No bets placed for hour {Hour}. Skipping prize distribution.", previousHour);
            return;
        }
        foreach (var entry in leaderboard)
        {
            await leaderboardRepository.AddLeaderboardEntryAsync(new LeaderboardEntry
            {
                PlayerId = entry.PlayerId,
                PlayerName = entry.PlayerName,
                TotalBetAmount = entry.TotalBets,
                Rank = entry.Rank,
                Hour = previousHour
            });
        }
        await DistributePrizes(leaderboard, previousHour);
    }

    private async Task DistributePrizes(List<LeaderboardEntryDto> leaderboard, DateTime hourCompleted)
    {
        if (leaderboard.Count == 0)
        {
            logger.LogInformation($"No players on leaderboard for hour {hourCompleted}");
            return;
        }

        try
        {
            await messagePublisher.InitializeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize MessagePublisher.");
            return;
        }

        foreach (var prizeConfig in _settings.PrizeConfiguration)
        {
            var winner = leaderboard.FirstOrDefault(e => e.Rank == prizeConfig.Rank);

            if (winner != null)
            {
                var prizeNotification = new Prize
                {
                    PlayerId = winner.PlayerId,
                    PlayerName = winner.PlayerName,
                    Amount = prizeConfig.Amount,
                    Hour = hourCompleted,
                    Rank = winner.Rank
                };

                logger.LogInformation($"Awarding prize to player {winner.PlayerName} (ID: {winner.PlayerId}) for rank {winner.Rank} with amount {prizeConfig.Amount}");

                try
                {
                    await messagePublisher.PublishMessageAsync(prizeNotification);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send prize notification for Player ID {winner.PlayerId}");
                }
                await prizeRepository.AddPrizeAsync(prizeNotification);
            }
        }
    }
}