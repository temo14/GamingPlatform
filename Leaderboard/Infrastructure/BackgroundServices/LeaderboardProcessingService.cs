using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

internal class LeaderboardProcessingService(
    IServiceScopeFactory scopeFactory,
    ILogger<LeaderboardProcessingService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using (var scope = scopeFactory.CreateAsyncScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
                    await service.ProcessHourlyLeaderboardAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing leaderboard.");
            }

            var now = DateTime.UtcNow;
            var nextHour = now.AddHours(1);
            var delay = nextHour - now;
            
            await Task.Delay(delay, stoppingToken);
        }
    }
}
