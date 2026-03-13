using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses;
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
        _artistCount = await UcCountArtists.ExecuteAsync();
        _albumCount = await UcCountAlbums.ExecuteAsync();
        _songCount = await UcCountSongs.ExecuteAsync();
        _genreCount = await UcCountGenres.ExecuteAsync();

        _latestAlbums = await UcGetLatestAlbums.ExecuteAsync(
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
        );

        await InvokeAsync(StateHasChanged);
    }
}