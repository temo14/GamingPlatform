using Application.Features.Game.DTOs;
using Application.Interfaces;

namespace Infrastructure.Services.Game;

internal class GameService : IGameService
{
    public GameResultDto PlayGame(int betAmount)
    {
        var random = new Random();
        bool win = random.Next(0, 2) == 1; // 50%

        var gameResult = new GameResultDto
        {
            BetAmount = betAmount,
            IsWin = win,
            WinAmount = win ? betAmount * 2 : 0
        };

        return gameResult;
    }
}
