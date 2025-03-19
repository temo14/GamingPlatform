using Application.Features.Compute.CurrentLeaderboard.DTOs;
using Domain.Entities;

namespace Application.Services;

public interface ILeaderboardService
{
    Task ProcessBetAsync(Bet betEvent);
    Task<List<LeaderboardEntryDto>> GetCurrentLeaderboardAsync();
    Task ProcessHourlyLeaderboardAsync();
}
