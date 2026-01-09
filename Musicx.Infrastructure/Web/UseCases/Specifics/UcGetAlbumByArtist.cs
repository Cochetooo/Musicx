using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumByArtist(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IGetAlbumByArtistUseCase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGetAlbumByArtist));
    
    public async Task<OutAlbumList> ExecuteAsync(long artistId, string query = "")
    {
        var endpoint = $"/api/albums/by-artist/{artistId}?query={query}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutAlbumList
            {
                Items = [],
                Total = 0,
                AverageRating = null,
            };
        }
        
        var json = JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutAlbumList
            {
                Items = [],
                Total = 0,
                AverageRating = null,
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutAlbumList Execute(long artistId, string query = "")
    {
        throw new NotImplementedException();
    }
}