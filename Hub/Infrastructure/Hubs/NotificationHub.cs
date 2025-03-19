using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.Hubs;

[Authorize]
public class NotificationHub(
    IUserConnectionManager connectionManager) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await connectionManager.AddConnectionAsync(userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await connectionManager.RemoveConnectionAsync(userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}