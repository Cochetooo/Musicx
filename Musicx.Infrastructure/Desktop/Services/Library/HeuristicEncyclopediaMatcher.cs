using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Infrastructure.Desktop.Services.Library;

public sealed class HeuristicEncyclopediaMatcher : IEncyclopediaMatcher
{
    public Task<EncyclopediaTrackMatch> MatchTrackAsync(LocalTrack localTrack, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var title = localTrack.Title.Trim();
        var artist = localTrack.Artist.Trim();
        var album = localTrack.Album.Trim();

        var found = !title.Contains("unknown", StringComparison.OrdinalIgnoreCase)
                    && !artist.Contains("unknown", StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(new EncyclopediaTrackMatch {
            ApiTrackId = found ? $"heuristic:{artist}:{title}" : null,
            ApiSongId = null,
            ApiAlbumId = null,
            ApiArtistId = null,
            CanonicalTitle = found ? title : null,
            CanonicalArtist = found ? artist : null,
            CanonicalAlbum = found ? album : null,
            Found = found
        });
    }
}