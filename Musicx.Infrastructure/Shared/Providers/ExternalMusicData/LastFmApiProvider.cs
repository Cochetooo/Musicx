using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class LastFmApiProvider(
    HttpClient httpClient,
    ILoggerProvider loggerProvider,
    IConfiguration configuration) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(LastFmApiProvider));
    
    private readonly string _apiKey = configuration["Apis:LastFm:ApiKey"]
        ?? throw new NullReferenceException("❌ Apis:LastFm:ApiKey is null");

    public async Task<OutArtist?> GetArtistInfoAsync(string name, CancellationToken ct = default)
        => null;

    public async Task<OutAlbum?> GetAlbumInfoAsync(string name, string artist, CancellationToken ct = default)
    {
        var endpoint = $"{configuration["Apis:LastFm:BaseUrl"]}?method=album.getinfo&artist={artist}&album={name}&api_key={_apiKey}&format=json";
        _logger.LogInformation($"🌍🏳️ LAST FM : GET {endpoint}");

        try
        {
            var response = await httpClient.GetStringAsync(endpoint, ct);
            _logger.LogDebug(response);
            
            var lastFmSearchAlbum = JsonSerializer.Deserialize<LastFmSearchAlbum.RootObject>(response);

            if (null == lastFmSearchAlbum)
            {
                _logger.LogWarning($"⚠️ LAST FM : No entry searching album for: {name}");
                return null;
            }

            var album = new OutAlbum
            {
                Name = name,
                ArtworkUrl = lastFmSearchAlbum.album.image[4]._text,
            };
            
            _logger.LogInformation($"🌍✅ LAST FM : GET {endpoint} - SUCCESS");
            return album;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"🌍❌ LAST FM : GET {endpoint} - ERROR: {ex.Message}");
            return null;
        }
    }
}