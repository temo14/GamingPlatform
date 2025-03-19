using Application.Services;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

internal class BetEventProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<BetEventProcessor> logger) : IBetEventProcessor
{
    public async Task ProcessBetEventAsync(string eventJson)
    {
        if (eventJson == null)
        {
            logger.LogWarning("Failed to deserialize bet event");
            return;
        }

        await using (var scope = scopeFactory.CreateAsyncScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

            var betEvent = JsonSerializer.Deserialize<Bet>(eventJson);

            try
            {
                if (betEvent is not null)
                {
                    await service.ProcessBetAsync(betEvent);
                    logger.LogInformation("--> Bet added!");
                }
                else
                {
                    logger.LogWarning("--> Bet is null");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"--> Could not save Bet: {ex.Message}");
            }
        }
    }
}