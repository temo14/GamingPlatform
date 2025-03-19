using Application.Features.Player.DTOs;
using Application.Services;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Player.Queries.Login;

internal class PlayerLoginQueryHandler(
    IPlayerRepository playerRepository,
    ITokenService tokenService,
    IUserConnectionManager presenceTracker) : IRequestHandler<PlayerLoginQuery, PlayerLoginDto>
{
    public async Task<PlayerLoginDto> Handle(PlayerLoginQuery request, CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetPlayerByUsernameAsync(request.Username, cancellationToken);

        if (player == null)
            throw new UnauthorizedAccessException("User Does not Exists!");

        if (!tokenService.IsValidCredentials(request, player))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = tokenService.GenerateToken(player);


        return new PlayerLoginDto { Token = token, PlayerId = player.Id };
    }
}
