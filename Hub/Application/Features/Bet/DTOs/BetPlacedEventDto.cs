namespace Application.Features.Bet.DTOs;

public class BetPlacedEventDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}