namespace Application.Services;

public interface IUserConnectionManager
{
    Task AddConnectionAsync(string userId, string connectionId);
    Task RemoveConnectionAsync(string userId, string connectionId);
    Task<List<string>> GetConnectionsAsync(string userId);
    Task<bool> IsUserOnlineAsync(string userId);
}
