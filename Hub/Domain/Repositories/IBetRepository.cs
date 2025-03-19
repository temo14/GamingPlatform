using Domain.Models;

namespace Domain.Repositories;

public interface IBetRepository
{
    Task SaveBetAsync(Bet betEvent, CancellationToken cancellationToken);
}
