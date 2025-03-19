namespace Domain.Entities;

public class Bet
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
