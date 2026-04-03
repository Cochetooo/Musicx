using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.UseCases;
using Musicx.Application.Shared.Models.Queries;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Desktop.Services.Library;

public sealed class WebApiEncyclopediaMatcher(IApiClient apiClient) : IEncyclopediaMatcher
{
    public async Task<EncyclopediaTrackMatch> MatchTrackAsync(LocalTrack localTrack, CancellationToken cancellationToken = default)
    {
        var filter = $"{localTrack.Artist} {localTrack.Title}";
        var songs = await apiClient.FindAsync<InSong, OutSong>(query: FindQuery<InSong>.Create(filter), pagingOptions: new PagingOptions(Take: 5, Skip: 0));
        var song = songs.Items.FirstOrDefault();

        if (song is null)
        {
            return new EncyclopediaTrackMatch();
        }

        return new EncyclopediaTrackMatch
        {
            ApiTrackId = song.Id.ToString(),
            ApiSongId = song.Id,
            ApiAlbumId = song.AlbumId,
            ApiArtistId = song.ArtistId,
            CanonicalTitle = song.Title,
            CanonicalArtist = song.Artist?.Name,
            CanonicalAlbum = song.Album?.Name,
            Found = true
        };
    }
}