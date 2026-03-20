using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class DeezerApiProvider(
    HttpClient httpClient,
    IConfiguration configuration,
    ILoggerProvider loggerProvider) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(DeezerApiProvider));
    
    public async Task<OutArtworkCandidate?> GetArtistArtworkAsync(string name, string normalizedName, CancellationToken ct = default)
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
            
            return IExternalMusicDataProvider.BuildCandidate(
                url: artist.ArtworkUrl,
                source: "Deezer",
                label: artist.Name,
                width: 1200,
                height: 1200,
                score: IExternalMusicDataProvider.ComputeScore(normalizedName, StringHelper.Normalize(artist.Name)) + 12
            );
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"🌍❌ DEEZER : GET {endpoint} - ERROR: {ex.Message}");
            return null;
        }
    }

    public async Task<OutArtworkCandidate?> GetAlbumArtworkAsync(string name, string artist,
        string normalizedName, string normalizedArtist, CancellationToken ct = default)
    {
        var endpoint = $"{configuration["Apis:Deezer:BaseUrl"]}search?q=album:\"{Uri.EscapeDataString(name)}\" artist:\"{Uri.EscapeDataString(artist)}\"";
        var json = await httpClient.GetStringAsync(endpoint, ct);
        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
        {
            return null;
        }

        var best = data.EnumerateArray()
            .Select(entry =>
            {
                var albumName = entry.GetProperty("album").GetProperty("title").GetString() ?? string.Empty;
                var artistName = entry.GetProperty("artist").GetProperty("name").GetString() ?? string.Empty;
                var url = entry.GetProperty("album").TryGetProperty("cover_xl", out var xl)
                    ? xl.GetString()
                    : entry.GetProperty("album").GetProperty("cover_big").GetString();
                var score = IExternalMusicDataProvider.ComputeAlbumScore(
                    normalizedName, normalizedArtist, StringHelper.Normalize(albumName), StringHelper.Normalize(artistName));

                return new { AlbumName = albumName, ArtistName = artistName, Url = url, Score = score };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Url))
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (best is null)
        {
            return null;
        }

        return IExternalMusicDataProvider.BuildCandidate(
            url: best.Url!, 
            source: "Deezer", 
            label: $"{best.ArtistName} — {best.AlbumName}", 
            width: 1200, 
            height: 1200, 
            score: best.Score + 12
        );
    }
}