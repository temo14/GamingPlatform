using Application.Features.Player.Commands.PlayerRegister;
using Application.Features.Player.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateAccessToken([FromBody] PlayerLoginQuery request, CancellationToken cancellationToken)
            => Ok(await mediator.Send(request, cancellationToken));

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCompany([FromBody] PlayerRegisterCommand request, CancellationToken cancellationToken)
        => Ok(await mediator.Send(request, cancellationToken));
    }
}
