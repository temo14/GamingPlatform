using MediatR;

namespace Application.Features.Compute.CurrentLeaderboard.Command;

public record ProcessLeaderboardCommand : IRequest<Unit>;