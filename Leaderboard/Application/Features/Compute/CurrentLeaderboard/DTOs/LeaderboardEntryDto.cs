namespace Application.Features.Compute.CurrentLeaderboard.DTOs;

public class LeaderboardEntryDto
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public decimal TotalBets { get; set; }
    public int Rank { get; set; }
}