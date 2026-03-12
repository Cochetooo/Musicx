using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;

namespace Musicx.Infrastructure.Web.UseCases.Genre;

public sealed class FindGenreDataViewService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IFindGenreDataViewService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindGenreDataViewService));

    public async Task<OutGenreDataView?> ExecuteAsync(long genreId, long? userId = null)
    {
        var endpoint = $"/api/genres/{genreId}/data-view";
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

        var json = JsonSerializer.Deserialize<OutGenreDataView>(response, JsonHelper.OptionsDefault);
        if (json is null)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Deserialization failed");
            return null;
        }

        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");
        return json;
    }
}