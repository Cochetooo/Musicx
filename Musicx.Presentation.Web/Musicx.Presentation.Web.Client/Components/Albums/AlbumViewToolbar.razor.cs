using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Enums;
using Musicx.Presentation.Web.Client.Pages.SingleView;

namespace Musicx.Presentation.Web.Client.Components.Albums;

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
    [Parameter] public (string, bool) SelectedSort { get; set; }
    [Parameter] public RatingMode? InitialRatingMode { get; set; }
    [Parameter] public ReleasesViewMode? InitialViewMode { get; set; }

    private string _searchText = string.Empty;
    private double _zoom = 1.2;
    private bool _groupBy;
    private ReleasesViewMode _viewMode = ReleasesViewMode.Grid;
    private RatingMode _selectedRatingMode = RatingMode.OutOfTen;

    protected override void OnParametersSet()
    {
        if (InitialRatingMode is not null)
        {
            _selectedRatingMode = InitialRatingMode.Value;
        }
        
        if (InitialViewMode is not null)
        {
            _viewMode = InitialViewMode.Value;
        }
    }

    void OnInput(string? v)
    {
        if (string.IsNullOrWhiteSpace(v) || v == _searchText)
        {
            return;
        }
        
        _searchText = v;
        OnSearch.InvokeAsync(v);
    }

    void OnSortChanged((string, bool) sort)
    {
        SelectedSort = sort;
        RaiseSort(sort);
    }

    void RaiseSort((string, bool) s)
    {
        SelectedSort = s;
        OnSort.InvokeAsync(s);
    }
    
    void ChangeView(ReleasesViewMode v)
    {
        _viewMode = v;
        OnViewModeChanged.InvokeAsync(v);
    }

    void ChangeRatingMode(RatingMode v)
    {
        _selectedRatingMode = v;
        OnRatingModeChanged.InvokeAsync(v);
    }

    void OnZoomValueChanged(double value)
    {
        _zoom = value;
        OnZoomChanged.InvokeAsync(value);
    }

    void OnGroupByValueChanged(bool value)
    {
        _groupBy = value;
        OnGroupByChanged.InvokeAsync(value);
    }

    private Variant GetViewVariant(ReleasesViewMode mode) => _viewMode == mode ? Variant.Filled : Variant.Outlined;
    private Color GetViewColor(ReleasesViewMode mode) => _viewMode == mode ? Color.Primary : Color.Default;
}