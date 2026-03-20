using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;
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

    public async Task<OutArtworkCandidate?> GetArtistArtworkAsync(string name, string normalizedName,
        CancellationToken ct = default)
    {
        var endpoint = $"{configuration["Apis:LastFm:BaseUrl"]}?method=artist.getinfo&artist={Uri.EscapeDataString(name)}&api_key={configuration["Apis:LastFm:ApiKey"]}&format=json";
        
        var json = await httpClient.GetStringAsync(endpoint, ct);
        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("artist", out var artist))
        {
            return null;
        }

        var artistName = artist.GetProperty("name").GetString() ?? name;
        var imageUrl = artist.TryGetProperty("image", out var images)
            ? images.EnumerateArray()
                .Select(i => i.GetProperty("#text").GetString())
                .LastOrDefault(url => !string.IsNullOrWhiteSpace(url))
            : null;

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        return IExternalMusicDataProvider.BuildCandidate(
            url: imageUrl!, 
            source: "Last.fm", 
            label: artistName, 
            width: 900, 
            height: 900, 
            score: IExternalMusicDataProvider.ComputeScore(normalizedName, StringHelper.Normalize(artistName)) + 4
        );
    }

    public async Task<OutArtworkCandidate?> GetAlbumArtworkAsync(string name, string artist, 
        string normalizedName, string normalizedArtist, CancellationToken ct = default)
    {
        var endpoint = $"{configuration["Apis:LastFm:BaseUrl"]}?method=album.getinfo&artist={Uri.EscapeDataString(artist)}" +
                       $"&album={Uri.EscapeDataString(name)}&api_key={_apiKey}&format=json";
        
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
                Artist = new OutArtist
                {
                    Name = lastFmSearchAlbum.album.artist
                },
                ArtworkUrl = lastFmSearchAlbum.album.image[4]._text,
            };
            
            _logger.LogInformation($"🌍✅ LAST FM : GET {endpoint} - SUCCESS");
            
            return IExternalMusicDataProvider.BuildCandidate(
                url: album.ArtworkUrl, 
                source: "Last.fm", 
                label: album.Name, 
                width: 900, 
                height: 900, 
                score: IExternalMusicDataProvider.ComputeAlbumScore(normalizedName, 
                    normalizedArtist,
                    StringHelper.Normalize(album.Name),
                    StringHelper.Normalize(album.Artist?.Name)) + 6
            );
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"🌍❌ LAST FM : GET {endpoint} - ERROR: {ex.Message}");
            return null;
        }
    }
}