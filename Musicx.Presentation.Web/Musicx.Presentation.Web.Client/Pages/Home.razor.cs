using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;

namespace Musicx.Presentation.Web.Client.Pages;

public partial class Home
{
    private long _artistCount;
    private long _albumCount;
    private long _songCount;
    private long _genreCount;

    private List<OutAlbum> _latestAlbums = [];

    protected override async Task OnParametersSetAsync()
    {
        _artistCount = await Api.CountAsync<InArtist, OutArtist>();
        _albumCount = await Api.CountAsync<InAlbum, OutAlbum>();
        _songCount = await Api.CountAsync<InSong, OutSong>();
        _genreCount = await Api.CountAsync<InGenre, OutGenre>();

        _latestAlbums = (await Api.FindAsync<InAlbum, OutAlbum>(
            joins: new AlbumJoinSpecification
            {
                IncludeArtist = true,
                IncludeStats = true
            },
            order: new AlbumOrderSpecification
            {
                OriginalReleaseDate = -1,
                Name = 2
            },
            pagingOptions: new PagingOptions(Take: 5, Skip: 0)
        )).Items;

        await InvokeAsync(StateHasChanged);
    }
}