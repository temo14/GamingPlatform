using Application.Services;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal class PrizeNotificationService(
    IHubContext<NotificationHub> hubContext,
    IUserConnectionManager connectionManager,
    ILogger<PrizeNotificationService> logger) : IPrizeNotificationService
{
    public async Task SendPrizeNotification(string playerId, decimal prize)
    {
        var connections = await connectionManager.GetConnectionsAsync(playerId);
        if (await connectionManager.IsUserOnlineAsync(playerId))
        {
            foreach (var connectionId in connections)
            {
                await hubContext.Clients.Client(connectionId).SendAsync("ReceivePrizeNotification", "You Won Prize!", prize);
                logger.LogInformation($"Prize notification sent to player {playerId} on {connections.Count} connections");
            }
        }
        else
        {
            logger.LogWarning($"Player {playerId} is offline. Prize notification not sent in real-time.");
        }
    }
}
