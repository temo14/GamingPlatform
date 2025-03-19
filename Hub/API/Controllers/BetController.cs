using Application.Features.Bet.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/bet")]
[ApiController]
[Authorize]
public class BetController : ControllerBase
{
    private readonly IMediator _mediator;

    public BetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterBet([FromQuery] int amount, CancellationToken cancellationToken)
    {
        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User?.FindFirst(ClaimTypes.Name)?.Value;

        if (userId == null || userName == null)
            return Unauthorized();

        var query = new RegisterBetCommand
        {
            PlayerId = Guid.Parse(userId),
            PlayerName = userName,
            Amount = amount,
            CreatedAt = DateTime.Now,
            GameId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        return Ok(await _mediator.Send(query, cancellationToken));
    }
}
