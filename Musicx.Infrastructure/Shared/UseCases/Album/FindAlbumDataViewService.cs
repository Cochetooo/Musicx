using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.Album;
using Musicx.Contracts.Dto.Responses.Specifics.Albums;

namespace Musicx.Infrastructure.Shared.UseCases.Album;

public sealed class FindAlbumDataViewService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IFindAlbumDataViewService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindAlbumDataViewService));

    public async Task<OutAlbumDataView?> ExecuteAsync(long albumId, long? userId = null)
    {
        var endpoint = $"/api/albums/{albumId}/data-view";
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

        var json = JsonSerializer.Deserialize<OutAlbumDataView>(response, JsonHelper.OptionsDefault);
        if (json is null)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Deserialization failed");
            return null;
        }

        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");
        return json;
    }
}