namespace Application.Features.Player.DTOs;

internal class PlayerRegisterDto
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; }
    public bool Success { get; set; }
}
