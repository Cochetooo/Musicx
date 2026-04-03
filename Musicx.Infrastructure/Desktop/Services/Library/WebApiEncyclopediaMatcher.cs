using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.UseCases;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Desktop.Services.Library;

public sealed class WebApiEncyclopediaMatcher(IFindAllService<InSong, OutSong> apiClient) : IEncyclopediaMatcher
{
    public async Task<EncyclopediaTrackMatch> MatchTrackAsync(LocalTrack localTrack, CancellationToken cancellationToken = default)
    {
        var filter = $"{localTrack.Artist} {localTrack.Title}";
        var songs = await apiClient.ExecuteAsync(filter: filter, pagingOptions: new PagingOptions(Take: 5, Skip: 0));
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