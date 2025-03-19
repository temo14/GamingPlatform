using Application.Features.Compute.CurrentLeaderboard.DTOs;
using MediatR;

namespace Application.Features.Compute.CurrentLeaderboard.Query;

public record CurrentLeaderboardQuery : IRequest<IEnumerable<LeaderboardEntryDto>>;