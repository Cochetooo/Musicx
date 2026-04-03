using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.UseCases.Song;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Shared.UseCases.Song;

public sealed class FindSongByAlbumService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IFindSongByAlbumService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(FindSongByAlbumService));
    
    public async Task<List<OutSong>> ExecuteAsync(
        long albumId,
        IJoinSpecification<InSong>? joinSpec = null,
        OrderSpecification<InSong>? orderSpec = null)
    {
        var endpoint = $"/api/songs/by-album/{albumId}?";
        endpoint += QueryStringHelper.SetUseCaseParameters(joinSpec, orderSpec);
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        var json = JsonSerializer.Deserialize<List<OutSong>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public List<OutSong> Execute(
        long albumId,
        IJoinSpecification<InSong>? joinSpec = null,
        OrderSpecification<InSong>? orderSpec = null)
    {
        throw new NotImplementedException();
    }
}