using Application.Features.Game.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/game")]
[ApiController]
[Authorize]
public class GameController(IMediator mediator) : ControllerBase
{
    [HttpPost("play")]
    public async Task<IActionResult> PlayGame([FromQuery] int betAmount)
    {
        var token = Request.Headers["Authorization"].ToString();
        var result = await mediator.Send(new PlayGameCommand { BetAmount = betAmount, Token = token });

        return Ok(result);
    }
}
