using Application.Features.Player.DTOs;
using MediatR;

namespace Application.Features.Player.Commands.PlayerRegister;

public class PlayerRegisterCommand : IRequest<PlayerRegisterDto>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
