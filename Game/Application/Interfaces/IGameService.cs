using Application.Features.Game.DTOs;

namespace Application.Interfaces;

public interface IGameService
{
    GameResultDto PlayGame(int betAmount);
}
