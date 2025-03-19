using Application.Features.Compute.CurrentLeaderboard.DTOs;
using Application.Services;
using MediatR;

namespace Application.Features.Compute.CurrentLeaderboard.Query;

internal class CurrentLeaderboardQueryHandler(
    ILeaderboardService service) : IRequestHandler<CurrentLeaderboardQuery, IEnumerable<LeaderboardEntryDto>>
{
    public async Task<IEnumerable<LeaderboardEntryDto>> Handle(CurrentLeaderboardQuery request, CancellationToken cancellationToken)
    {
        return await service.GetCurrentLeaderboardAsync();
    }
}