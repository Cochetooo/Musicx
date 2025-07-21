using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumByArtist(
    HttpClient httpClient,
    ILoggerProvider loggerProvider) : IGetAlbumByArtistUseCase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGetAlbumByArtist));

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task<List<OutAlbum>> ExecuteAsync(long artistId)
    {
        var endpoint = $"/api/albums/by-artist/{artistId}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        var json = JsonSerializer.Deserialize<List<OutAlbum>>(response, options);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public List<OutAlbum> Execute(long artistId)
    {
        throw new NotImplementedException();
    }
}