using Application.Services;
using StackExchange.Redis;

namespace Infrastructure.Services;

internal class UserConnectionManager(
    IConnectionMultiplexer redis) : IUserConnectionManager
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task AddConnectionAsync(string userId, string connectionId)
    {
        await _database.SetAddAsync($"connections:{userId}", connectionId);
    }

    public async Task RemoveConnectionAsync(string userId, string connectionId)
    {
        await _database.SetRemoveAsync($"connections:{userId}", connectionId);

        if (await _database.SetLengthAsync($"connections:{userId}") == 0)
        {
            await _database.KeyDeleteAsync($"connections:{userId}");
        }
    }
    public async Task<List<string>> GetConnectionsAsync(string userId)
    {
        var connections = await _database.SetMembersAsync($"connections:{userId}");
        return connections.Select(c => c.ToString()).ToList();
    }

    public async Task<bool> IsUserOnlineAsync(string userId)
    {
        return await _database.KeyExistsAsync($"connections:{userId}");
    }
}
