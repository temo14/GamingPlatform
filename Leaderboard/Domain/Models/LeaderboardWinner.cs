namespace Domain.Entities;

public class LeaderboardEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public decimal TotalBetAmount { get; set; }
    public int Rank { get; set; }
    public DateTime Hour { get; set; }
}
