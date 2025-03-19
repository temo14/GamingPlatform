using Application.Features.Compute.CurrentLeaderboard.DTOs;
using Domain.Entities;

namespace Application.Services;

public interface ILeaderboardAggregator
{
    Task AddBetAsync(Bet betEvent);
    Task<List<LeaderboardEntryDto>> GetCurrentHourLeaderboardAsync();
    Task<List<LeaderboardEntryDto>> GetHourLeaderboardAsync(DateTime hour);
}