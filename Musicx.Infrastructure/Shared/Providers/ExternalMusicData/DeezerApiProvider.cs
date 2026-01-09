using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class DeezerApiProvider(
    HttpClient httpClient,
    IConfiguration configuration,
    ILoggerProvider loggerProvider) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(DeezerApiProvider));
    
    public async Task<OutArtist?> GetArtistInfoAsync(string name, CancellationToken ct = default)
    {
        var endpoint = $"{configuration["Apis:Deezer:BaseUrl"]}search?q=artist:\"{Uri.EscapeDataString(name)}\"";
        _logger.LogInformation($"🌍🏳️ DEEZER : GET {endpoint}️");

        try
        {
            var response = await httpClient.GetStringAsync(endpoint, ct);
            
            var deezerSearchArtist = JsonSerializer.Deserialize<DeezerSearchArtist.RootObject>(response, JsonHelper.OptionsDefault);

            if (null == deezerSearchArtist || 0 == deezerSearchArtist.data.Length)
            {
                _logger.LogWarning($"⚠️ DEEZER : No entry searching artist for: {name}");
                return null;
            }

            DeezerSearchArtist.Data bestData = deezerSearchArtist.data[0];
            int bestScore = int.MaxValue;

            foreach (var data in deezerSearchArtist.data)
            {
                var score = StringHelper.LevenshteinDistance(data.artist.name.ToLower(), name.ToLower());

                if (score < bestScore)
                {
                    bestScore = score;
                    bestData = data;
                }
            }

            var artist = new OutArtist
            {
                Name = name,
                ArtworkUrl = bestData.artist.picture_big
            };
                        
            _logger.LogDebug("ℹ️ DEEZER : Artwork Url Info : " + artist.ArtworkUrl);
            
            _logger.LogInformation($"🌍✅ DEEZER : GET {endpoint} - SUCCESS");
            return artist;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"🌍❌ DEEZER : GET {endpoint} - ERROR: {ex.Message}");
            return null;
        }
    }

    public async Task<OutAlbum?> GetAlbumInfoAsync(string name, string artist, CancellationToken ct = default)
        => null;
}