namespace Musicx.Presentation.Web.Client.Pages;

public partial class Home
{
    private long _artistCount;
    private long _albumCount;
    private long _songCount;
    private long _genreCount;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _artistCount = await UcCountArtists.ExecuteAsync();
        _albumCount = await UcCountAlbums.ExecuteAsync();
        _songCount = await UcCountSongs.ExecuteAsync();
        _genreCount = await UcCountGenres.ExecuteAsync();

        await InvokeAsync(StateHasChanged);
    }
}