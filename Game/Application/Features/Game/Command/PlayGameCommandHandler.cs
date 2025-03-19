using Application.Features.Game.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Game.Command;

internal class PlayGameCommandHandler(
    IGameService gameService,
    IHubClientService hubService) : IRequestHandler<PlayGameCommand, GameResultDto>
{
    public async Task<GameResultDto> Handle(PlayGameCommand request, CancellationToken cancellationToken)
    {
        var gameResult = gameService.PlayGame(request.BetAmount);

        bool success = await hubService.RegisterBetAsync(gameResult, request.Token, cancellationToken);
        if (!success)
            throw new Exception("Failed to register bet in Hub Service");

        return gameResult;
    }
}
