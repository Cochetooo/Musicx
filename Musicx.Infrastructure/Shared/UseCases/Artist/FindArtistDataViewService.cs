using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;

namespace Musicx.Infrastructure.Shared.UseCases.Artist;

public sealed class FindArtistDataViewService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IFindArtistDataViewService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindArtistDataViewService));

    public async Task<OutArtistDataView?> ExecuteAsync(long genreId, long? userId = null)
    {
        var endpoint = $"/api/artists/{genreId}/data-view";
        if (userId.HasValue)
        {
            endpoint += $"?userId={userId.Value}";
        }

        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }

        var json = JsonSerializer.Deserialize<OutArtistDataView>(response, JsonHelper.OptionsDefault);
        if (json is null)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Deserialization failed");
            return null;
        }

        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");
        return json;
    }
}