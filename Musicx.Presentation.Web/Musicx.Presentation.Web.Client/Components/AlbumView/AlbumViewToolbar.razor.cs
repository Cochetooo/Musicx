using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Enums;
using Musicx.Presentation.Web.Client.Pages.SingleView;

namespace Musicx.Presentation.Web.Client.Components.AlbumView;

public partial class AlbumViewToolbar
{
    [Parameter] public EventCallback<string> OnSearch { get; set; }
    [Parameter] public EventCallback<(string,bool)> OnSort { get; set; }
    [Parameter] public EventCallback<ReleasesViewMode> OnViewModeChanged { get; set; }
    [Parameter] public EventCallback<double> OnZoomChanged { get; set; }
    [Parameter] public EventCallback<bool> OnGroupByChanged { get; set; }
    [Parameter] public EventCallback<RatingMode> OnRatingModeChanged { get; set; }
    [Parameter] public EventCallback OnNewRelease { get; set; }
    [Parameter] public bool AllowAdd { get; set; }
    [Parameter] public IEnumerable<(string key, string display)> AvailableSorts { get; set; } = 
    [
        ("ReleaseDate","Release Date ↓"),
        ("ReleaseDate","Release Date ↑"),
        
    ];
    [Parameter] public (string, bool) SelectedSort { get; set; }

    private string _searchText = string.Empty;
    private double _zoom = 1.0;
    private bool _groupBy;
    private ReleasesViewMode _viewMode = ReleasesViewMode.Grid;
    private RatingMode _selectedRatingMode = RatingMode.OutOfTen;

    void OnInput(string v) { _searchText = v; OnSearch.InvokeAsync(v); }

    void RaiseSort((string, bool) s)
    {
        SelectedSort = s;
        OnSort.InvokeAsync(s);
    }
    void ChangeView(ReleasesViewMode v) { _viewMode = v; OnViewModeChanged.InvokeAsync(v); }
    void ChangeRatingMode(RatingMode v) { _selectedRatingMode = v; OnRatingModeChanged.InvokeAsync(v); }
}