using MudBlazor;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Presentation.Web.Client.Pages.Charts;

public enum AlbumChartViewMode
{
    Grid,
    List
}

public partial class AlbumChart
{
    private OutAlbumList _outAlbumList = new();
    private AlbumChartQuery _query = new();
    private bool _isLoading;
    private bool _showRankOnList = true;
    private AlbumChartViewMode _viewMode = AlbumChartViewMode.Grid;
    private int _releaseYearRange = 1900;
    private int _upperReleaseYearRange = DateTime.UtcNow.Year;

    protected override async Task OnInitializedAsync()
    {
        SyncReleaseDates();
        await LoadChart();
    }

    private async Task LoadChart()
    {
        _isLoading = true;
        _query.MinDate = new DateTime(_releaseYearRange, 1, 1);
        _query.MaxDate = new DateTime(_upperReleaseYearRange, 12, 31);

        _outAlbumList = await Api.FindAlbumsByChartAsync(_query);
        _isLoading = false;
    }

    private void SyncReleaseDates()
    {
        _releaseYearRange = _query.MinDate?.Year ?? 1970;
        _upperReleaseYearRange = _query.MaxDate?.Year ?? DateTime.UtcNow.Year;
    }
}