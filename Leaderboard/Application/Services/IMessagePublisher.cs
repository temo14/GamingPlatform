using Domain.Entities;

namespace Application.Services;

public interface IMessagePublisher
{
    Task InitializeAsync();
    Task PublishMessageAsync(Prize message);
}
