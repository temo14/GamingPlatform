using Application.Features.Game.DTOs;
using MediatR;

namespace Application.Features.Game.Command;

public record PlayGameCommand : IRequest<GameResultDto>
{
    public required string Token { get; set; }
    public required int BetAmount { get; set; }
}