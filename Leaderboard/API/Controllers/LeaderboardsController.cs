using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Compute.CurrentLeaderboard.Query;
using Application.Features.Compute.CurrentLeaderboard.Command;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Route("api/leaderboard")]
[ApiController]
[Authorize]
public class LeaderboardsController(IMediator mediator) : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentLeaderboard()
        => Ok(await mediator.Send(new CurrentLeaderboardQuery()));

    [HttpPost("process")]
    public async Task<IActionResult> ProcessLeaderboard()
        => Ok(await mediator.Send(new ProcessLeaderboardCommand()));
}
