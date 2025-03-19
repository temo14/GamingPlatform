using Application.Features.Player.DTOs;
using MediatR;

namespace Application.Features.Player.Queries.Login;

public class PlayerLoginQuery : IRequest<PlayerLoginDto>
{
    public string Username { get; set; }
    public string Password { get; set; }
}