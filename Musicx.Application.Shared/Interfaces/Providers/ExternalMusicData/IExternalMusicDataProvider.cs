using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;

namespace Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;

public interface IExternalMusicDataProvider
{
    Task<OutArtworkCandidate?> GetArtistArtworkAsync(string name, string normalizedName, CancellationToken ct = default);
    Task<OutArtworkCandidate?> GetAlbumArtworkAsync(string name, string artist,
        string normalizedName, string normalizedArtist, CancellationToken ct = default);
    
    protected static OutArtworkCandidate BuildCandidate(string url, string source, string label, int? width, int? height, double score)
        => new()
        {
            Url = url,
            Source = source,
            Label = label,
            Width = width,
            Height = height,
            Score = Math.Round(score, 1),
            IsAnimated = IsAnimatedUrl(url)
        };
    
    protected static double ComputeScore(string expected, string actual)
    {
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(actual))
        {
            return 0;
        }

        var distance = StringHelper.LevenshteinDistance(expected, actual);
        var longest = Math.Max(expected.Length, actual.Length);
        return Math.Max(0, 100 - (distance * 100d / Math.Max(1, longest)));
    }

    protected static double ComputeAlbumScore(string expectedAlbum, string expectedArtist, string actualAlbum, string actualArtist)
        => (ComputeScore(expectedAlbum, actualAlbum) * 0.65) + (ComputeScore(expectedArtist, actualArtist) * 0.35);
    
    private static bool IsAnimatedUrl(string? url)
        => !string.IsNullOrWhiteSpace(url)
           && (url.Contains(".gif", StringComparison.OrdinalIgnoreCase)
               || url.Contains("format=gif", StringComparison.OrdinalIgnoreCase)
               || url.Contains("animated", StringComparison.OrdinalIgnoreCase));
}