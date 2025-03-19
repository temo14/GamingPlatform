namespace Domain.Models;

public class Bet
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public bool IsWin { get; set; }
    public DateTime CreatedAt { get; set; }
}
