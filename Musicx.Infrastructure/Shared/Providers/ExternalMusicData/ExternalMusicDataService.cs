using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class ExternalMusicDataService(
    IEnumerable<IExternalMusicDataProvider> providers,
    ILoggerProvider loggerProvider)
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ExternalMusicDataService));

    public async Task<OutArtworkSearchResult> GetArtistArtworkAsync(string name, CancellationToken token = default)
    {
        var candidates = new List<OutArtworkCandidate>();
        var normalizedName = StringHelper.Normalize(name);
        
        foreach (var provider in providers)
        {
            try
            {
                _logger.LogDebug("🔌 Get Artist Artwork : Try with " + provider.GetType().Name);
                var result = await provider.GetArtistArtworkAsync(name, normalizedName, token);
                if (result is not null)
                {
                    candidates.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Get Artist Artwork : An error occured during search: " + ex.Message);
                return new OutArtworkSearchResult
                {
                    Candidates = []
                };
            }
        }
        
        return new OutArtworkSearchResult
        {
            Candidates = RankAndTrim(candidates)
        };
    }

    public async Task<OutArtworkSearchResult> GetAlbumArtworkAsync(string name, string artist, CancellationToken ct = default)
    {
        var candidates = new List<OutArtworkCandidate>();
        var normalizedName = StringHelper.Normalize(name);
        var normalizedArtist = StringHelper.Normalize(artist);
        
        foreach (var provider in providers)
        {
            try
            {
                _logger.LogDebug("🔌 Get Album Artwork : Try with " + provider.GetType().Name);
                var result = await provider.GetAlbumArtworkAsync(name, artist, normalizedName, normalizedArtist, ct);
                if (result is not null)
                {
                    candidates.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Get Album Artwork : An error occured during search: " + ex.Message);
                return new OutArtworkSearchResult
                {
                    Candidates = []
                };
            }
        }
        
        return new OutArtworkSearchResult
        {
            Candidates = RankAndTrim(candidates)
        };
    }
    
    private IReadOnlyList<OutArtworkCandidate> RankAndTrim(IEnumerable<OutArtworkCandidate> candidates)
        => candidates
            .Where(candidate => !string.IsNullOrWhiteSpace(candidate.Url))
            .GroupBy(candidate => candidate.Url)
            .Select(group => group.OrderByDescending(x => x.Score).First())
            .OrderByDescending(candidate => candidate.Score)
            .Take(3)
            .ToList();


}