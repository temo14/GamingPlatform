namespace Application.Features.Game.DTOs;

public class GameResultDto
{
    public decimal BetAmount { get; set; }
    public bool IsWin { get; set; }
    public decimal WinAmount { get; set; }
}
