using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class GetSongByAlbumService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IGetSongByAlbumService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(GetSongByAlbumService));
    
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