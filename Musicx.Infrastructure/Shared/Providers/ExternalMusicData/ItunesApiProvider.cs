using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class ItunesApiProvider(
    HttpClient httpClient,
    ILoggerProvider loggerProvider,
    IConfiguration configuration) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ItunesApiProvider));

    public async Task<OutArtworkCandidate?> GetArtistArtworkAsync(
        string name, string normalizedName, CancellationToken ct = default)
        => null;

    public async Task<OutArtworkCandidate?> GetAlbumArtworkAsync(string name, string artist,
        string normalizedName, string normalizedArtist, CancellationToken ct = default)
    {
        var endpoint = $"{configuration["Apis:Itunes:BaseUrl"]}search?entity=album&limit=10&term={Uri.EscapeDataString($"{artist} {name}")}";
        var json = await httpClient.GetStringAsync(endpoint, ct);
        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
        {
            return null;
        }

        var best = results.EnumerateArray()
            .Select(entry =>
            {
                var albumName = entry.TryGetProperty("collectionName", out var albumProperty)
                    ? albumProperty.GetString() ?? string.Empty
                    : string.Empty;
                var artistName = entry.TryGetProperty("artistName", out var artistProperty)
                    ? artistProperty.GetString() ?? string.Empty
                    : string.Empty;
                var url = entry.TryGetProperty("artworkUrl100", out var artwork)
                    ? artwork.GetString()
                    : null;
                var upgradedUrl = string.IsNullOrWhiteSpace(url)
                    ? null
                    : url.Replace("100x100bb", "1200x1200bb").Replace("100x100", "1200x1200");
                var score = IExternalMusicDataProvider.ComputeAlbumScore(
                    normalizedName, normalizedArtist, StringHelper.Normalize(albumName), StringHelper.Normalize(artistName));

                return new { AlbumName = albumName, ArtistName = artistName, Url = upgradedUrl, Score = score };
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
            source: "iTunes", 
            label: $"{best.ArtistName} — {best.AlbumName}", 
            width: 1200, 
            height: 1200, 
            score: best.Score + 10
        );
    }
}