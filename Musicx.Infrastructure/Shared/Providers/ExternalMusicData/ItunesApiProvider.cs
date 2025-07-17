using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class ItunesApiProvider(
    HttpClient httpClient,
    ILoggerProvider loggerProvider,
    IConfiguration configuration) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ItunesApiProvider));

    public async Task<OutArtist?> GetArtistInfoAsync(string name, CancellationToken ct = default)
    {
        var endpoint = $"/search?term={name}&entity=musicArtist";
        _logger.LogInformation($"🌍🏳️ ITUNES : GET {endpoint}");

        try
        {
            var response = await httpClient.GetStringAsync(endpoint, ct);
            throw new NotImplementedException();
            var itunesSearchArtist = JsonSerializer.Deserialize<LastFmSearchArtist.RootObject>(response);

            if (null == itunesSearchArtist)
            {
                _logger.LogWarning($"⚠️ Itunes : No entry searching artist for: {name}");
                return null;
            }

            var artist = new OutArtist
            {
                Name = name,
                ArtworkUrl = itunesSearchArtist.artist.image[3]._text,
            };

            _logger.LogInformation($"🌍✅ LAST FM : GET {endpoint} - SUCCESS");
            return artist;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"🌍❌ ITUNES : GET {endpoint} - ERROR: {ex.Message}");
            return null;
        }
    }
}