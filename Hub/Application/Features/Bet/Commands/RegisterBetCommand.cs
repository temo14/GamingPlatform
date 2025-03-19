using Application.Features.Bet.DTOs;
using MediatR;

namespace Application.Features.Bet.Commands;

public class RegisterBetCommand : IRequest<BetRegisterDto>
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? CreatedAt { get; set; }
}
