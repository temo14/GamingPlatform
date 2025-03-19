using Application.Services;
using MediatR;

namespace Application.Features.Compute.CurrentLeaderboard.Command;

internal class ProcessLeaderboardCommandHandler(
    ILeaderboardService service) : IRequestHandler<ProcessLeaderboardCommand, Unit>
{
    public async Task<Unit> Handle(ProcessLeaderboardCommand request, CancellationToken cancellationToken)
    {
        await service.ProcessHourlyLeaderboardAsync();
        return Unit.Value;
    }
}
