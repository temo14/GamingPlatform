using Application.Features.Bet.DTOs;
using Application.Services;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Bet.Commands;

internal class RegisterBetCommandHandler(
    IBetRepository betRepository,
    IMessagePublisher publisher) : IRequestHandler<RegisterBetCommand, BetRegisterDto>
{
    public async Task<BetRegisterDto> Handle(RegisterBetCommand request, CancellationToken cancellationToken)
    {
        var bet = new Domain.Models.Bet
        {
            Id = Guid.NewGuid(),
            PlayerId = request.PlayerId,
            PlayerName = request.PlayerName,
            GameId = request.GameId,
            Amount = request.Amount,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow
        };
        await betRepository.SaveBetAsync(bet, cancellationToken);

        await publisher.InitializeAsync();
        await publisher.PublishMessageAsync(new BetPlacedEventDto
        {
            Id = bet.Id,
            PlayerId = bet.PlayerId,
            GameId = bet.GameId,
            PlayerName = bet.PlayerName,
            Amount = bet.Amount,
            CreatedAt = bet.CreatedAt
        });

        return new BetRegisterDto
        {
            BetId = bet.Id,
            Success = true
        };
    }
}
