using Domain.Entities;

namespace Domain.Repositories;

public interface IPrizeRepository
{
    Task AddPrizeAsync(Prize prize);
}
