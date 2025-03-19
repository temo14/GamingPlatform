namespace Application.Services;

public interface IPrizeNotificationService
{
    Task SendPrizeNotification(string playerId, decimal prize);
}
