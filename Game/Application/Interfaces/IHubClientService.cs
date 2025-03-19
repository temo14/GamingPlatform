using Application.Features.Game.DTOs;

namespace Application.Interfaces;

public interface IHubClientService
{
    Task<bool> RegisterBetAsync(GameResultDto gameResult, string token, CancellationToken cancellationToken);
}
