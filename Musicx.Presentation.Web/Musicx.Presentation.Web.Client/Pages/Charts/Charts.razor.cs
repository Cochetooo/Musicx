using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Presentation.Web.Client.Pages.Charts;

public enum ChartsTab
{
    TopAlbums,
    TopArtists,
    TopGenres,
    TopSongs
}

public partial class Charts
{
    private readonly IReadOnlyList<(ChartsTab Tab, string Label)> _tabs =
    [
        (ChartsTab.TopAlbums, "Top Albums"),
        (ChartsTab.TopArtists, "Top Artistes"),
        (ChartsTab.TopGenres, "Top Genres"),
        (ChartsTab.TopSongs, "Top Morceaux")
    ];
    
    private ChartsTab _activeTab = ChartsTab.TopAlbums;

    private void SetTab(ChartsTab tab) => _activeTab = tab;
}