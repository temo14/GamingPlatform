using Application.Features.Bet.DTOs;

namespace Application.Services;

public interface IMessagePublisher
{
    Task InitializeAsync();
    Task PublishMessageAsync(BetPlacedEventDto message);
}
