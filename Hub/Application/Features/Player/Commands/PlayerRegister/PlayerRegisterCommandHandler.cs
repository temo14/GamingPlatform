using Application.Features.Player.DTOs;
using MediatR;
using Domain.Repositories;

namespace Application.Features.Player.Commands.PlayerRegister;

internal class PlayerRegisterCommandHandler(
    IPlayerRepository playerRepository) : IRequestHandler<PlayerRegisterCommand, PlayerRegisterDto>
{
    public async Task<PlayerRegisterDto> Handle(PlayerRegisterCommand request, CancellationToken cancellationToken)
    {
        if (await playerRepository.GetPlayerByUsernameAsync(request.Username, cancellationToken) is not null)
            throw new Exception("Username already exists");

        var player = new Domain.Models.Player
        {
            Username = request.Username,
            Password = request.Password
        };

        await playerRepository.AddPlayerAsync(player, cancellationToken);

        return new PlayerRegisterDto
        {
            PlayerId = player.Id,
            Username = player.Username,
            Success = true
        };
    }
}
