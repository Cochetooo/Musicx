using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Utilities;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetSongByAlbum(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IGetSongByAlbumUseCase
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(UcGetSongByAlbum));
    
    public async Task<List<OutSong>> ExecuteAsync(long albumId, string query = "")
    {
        var endpoint = $"/api/songs/by-album/{albumId}?query={query}";
        
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

    public List<OutSong> Execute(long albumId, string query = "")
    {
        throw new NotImplementedException();
    }
}