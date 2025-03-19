using Application.Features.Game.DTOs;
using Application.Interfaces;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Hub;

internal class HubClientService(
    IOptions<HubSettings> options,
    IHttpClientFactory clientFactory) : IHubClientService
{
    public async Task<bool> RegisterBetAsync(GameResultDto gameResult, string token, CancellationToken cancellationToken)
    {
        var hubSettings = options.Value;
        using var client = clientFactory.CreateClient("HubClient");
        client.DefaultRequestHeaders.Add("Authorization", token);
        var endpoint = $"{hubSettings.BetEndPoint}?amount={gameResult.BetAmount}";

        var response = await client.PostAsync(endpoint, null, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return false;

        return true;
    }
}