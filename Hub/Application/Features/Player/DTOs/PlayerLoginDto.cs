namespace Application.Features.Player.DTOs;

internal class PlayerLoginDto
{
    public Guid PlayerId { get; set; }
    public required string Token { get; set; }
}
