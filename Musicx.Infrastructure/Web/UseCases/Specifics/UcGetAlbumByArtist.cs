using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Utilities;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumByArtist(
    HttpClient httpClient,
    ILoggerProvider loggerProvider) : IGetAlbumByArtistUseCase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGetAlbumByArtist));
    
    public async Task<List<OutAlbum>> ExecuteAsync(long artistId, string query = "")
    {
        var endpoint = $"/api/albums/by-artist/{artistId}?query={query}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        var json = JsonSerializer.Deserialize<List<OutAlbum>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public List<OutAlbum> Execute(long artistId, string query = "")
    {
        throw new NotImplementedException();
    }
}