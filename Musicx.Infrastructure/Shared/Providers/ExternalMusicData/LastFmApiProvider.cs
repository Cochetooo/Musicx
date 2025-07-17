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
    {
        var endpoint = $"{configuration["Apis:LastFm:BaseUrl"]}?method=artist.getinfo&artist={name}&api_key={_apiKey}&format=json";
        _logger.LogInformation($"🌍🏳️ LAST FM : GET {endpoint}");

        try
        {
            var response = await httpClient.GetStringAsync(endpoint, ct);
            _logger.LogInformation(response);
            
            var lastFmSearchArtist = JsonSerializer.Deserialize<LastFmSearchArtist.RootObject>(response);

            if (null == lastFmSearchArtist)
            {
                _logger.LogWarning($"⚠️ LAST FM : No entry searching artist for: {name}");
                return null;
            }

            var artist = new OutArtist
            {
                Name = name,
                ArtworkUrl = lastFmSearchArtist.artist.image[2]._text,
            };
            
            _logger.LogInformation($"🌍✅ LAST FM : GET {endpoint} - SUCCESS");
            return artist;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"🌍❌ LAST FM : GET {endpoint} - ERROR: {ex.Message}");
            return null;
        }
    }
}