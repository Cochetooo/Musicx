using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;
using Musicx.Presentation.Web.Client.Modals.Admin.Artists;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public enum ReleasesViewMode
{
    List,
    Grid,
    Timeline
}

public partial class ArtistView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private ArtistEditModal _artistEditModal = null!;
    private AlbumEditModal _albumEditModal = null!;
    
    private OutArtist? _artist;
    private OutAlbumList _artistAlbums = new();
    private List<OutAlbum> _filteredAlbums = [];
    private OutGenericList<OutUserAlbumAttribute>? _userAttrs;
    private Dictionary<ReleaseType, bool> _availableReleaseTypes = [];
    private Dictionary<int, int> _releaseCountPerYears = [];
    
    private ReleasesViewMode _viewMode = ReleasesViewMode.List;
    private bool _groupByType = true;
    private double _zoomLevel = 1.2;
    private (string sortBy, bool asc) _selectedSort = ("ReleaseDate", false);
    
    private readonly List<ChartSeries> _historySeries = [];
    private readonly ChartOptions _historyChartOptions = new()
    {
        InterpolationOption = InterpolationOption.Periodic,
        MaxNumYAxisTicks = 20,
        YAxisTicks = 1
    };
    private readonly AxisChartOptions _axisChartOptions = new()
    {
        MatchBoundsToSize = true,
    };
    private string[] _xAxisChartLabels = [];

    protected override async Task OnParametersSetAsync()
        => await LoadArtist();

    protected override void OnInitialized()
    => _logger = LoggerProvider.CreateLogger(nameof(ArtistView));

    private async Task LoadArtist()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Artist ID is null or empty.");
            return;
        }

        if (!long.TryParse(Id, out var artistId))
        {
            _logger.LogError("❌ Invalid Artist ID format: {Id}", Id);
            return;
        }
        
        _artist = await UcGet.ExecuteAsync(artistId);

        if (_artist is null)
        {
            _logger.LogError("⚠️ No artist found for ID: {Id}", Id);
            return;
        }

        _logger.LogInformation($"✅ Artist loaded: {_artist.Name} ({_artist.Id})");
        await InvokeAsync(StateHasChanged);
        
        _artistAlbums = await UcGetAlbums.ExecuteAsync(_artist.Id, joins: new AlbumJoinSpecification
        {
            IncludePrimaryGenres = true,
            IncludeInfluenceGenres = true,
            IncludeStats = true
        }, order: new AlbumOrderSpecification
        {
            OriginalReleaseDate = 1,
            Name = 2
        });
        _filteredAlbums = new List<OutAlbum>(_artistAlbums.Items);
        _logger.LogInformation("🎵 Retrieved {Count} albums for artist {ArtistId}", _artistAlbums.Total, _artist.Id);
        
        _availableReleaseTypes = _artistAlbums
            .Items
            .Where(s => s.ReleaseType.HasValue)
            .Select(s => s.ReleaseType!.Value)
            .Distinct()
            .ToDictionary(r => r, r => r is ReleaseType.Lp or ReleaseType.MixTape or ReleaseType.Soundtrack);
        
        await InvokeAsync(StateHasChanged);

        if (UserClientContext.CurrentUser is not null)
        {
            _userAttrs = await UcGetUserRatings.ExecuteAsync(
                userId: UserClientContext.CurrentUser.Id,
                artistId: _artist.Id,
                pagingOptions: new PagingOptions(Take: 100_000, Skip: 0)
            );
        }
     
        CalculateReleasesPerYear();
        UpdateChart();
        
        await InvokeAsync(StateHasChanged);
        
        _logger.LogInformation($"✅ Chart created.");
    }

    private void OnSearch(string text)
    {
        _filteredAlbums = _artistAlbums.Items
            .Where(a => a.Name.ToLower().Contains(text.ToLower()))
            .ToList();
    }

    private void OnSort((string key, bool descending) sort)
    {
        if (sort.descending)
        {
            switch (sort.key)
            {
                case "Name":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.Name).ToList();
                    break;
                case "NbRating":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.Stats?.Count).ToList();
                    break;
                case "GlobalRating":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.Stats?.Average).ToList();
                    break;
                case "MyRating":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => _userAttrs?
                        .Items
                        .FirstOrDefault(i => i.Album.Id == a.Id)
                        ?.Rating).ToList();
                    break;
                case "ReleaseDate":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.OriginalReleaseDate).ToList();
                    break;
            }
        }
        else
        {
            switch (sort.key)
            {
                case "Name":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.Name).ToList();
                    break;
                case "NbRating":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.Stats?.Count).ToList();
                    break;
                case "GlobalRating":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.Stats?.Average).ToList();
                    break;
                case "MyRating":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => _userAttrs?
                        .Items
                        .FirstOrDefault(i => i.Album.Id == a.Id)
                        ?.Rating).ToList();
                    break;
                case "ReleaseDate":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.OriginalReleaseDate).ToList();
                    break;
            }
        }

        _selectedSort = sort;
        StateHasChanged();
    }
    
    private void OnViewModeChanged(ReleasesViewMode vm) => _viewMode = vm;
    private void OnZoomChanged(double zoom) => _zoomLevel = zoom;
    private void OnToggleGroup(bool toggle) => _groupByType = toggle;

    private void OnRatingModeChanged(RatingMode mode)
    {
        if (UserClientContext.CurrentUser is not null)
        {
            UserClientContext.CurrentUser.PrefRatingMode = mode;
            // @TODO Persist change
        }
    }
    
    private void UpdateChart()
    {
        _historySeries.Clear();
        _historySeries.Add(new ChartSeries
        {
            Name = "# of releases",
            Data = _releaseCountPerYears.Select(g => (double) g.Value).ToArray()
        });
        
        _xAxisChartLabels = _releaseCountPerYears
            .Keys
            .Select(y => y.ToString())
            .ToArray();
    }
    
    private void CalculateReleasesPerYear()
    {
        if (null == _artist)
        {
            _logger.LogWarning("⚠️ Cannot calculate releases: artist is null");
            return;
        }
        
        var albumsWithDate = _artistAlbums
            .Items
            .Where(a => a is
            {
                OriginalReleaseDate: not null, 
                ReleaseType: ReleaseType.Lp or ReleaseType.Ep or ReleaseType.MixTape or ReleaseType.Soundtrack
            })
            .ToList();

        if (0 == albumsWithDate.Count)
        {
            _logger.LogWarning("⚠️ Cannot calculate releases: No album with release date.");
            _releaseCountPerYears.Clear();
            return;
        }

        int minYear = albumsWithDate.Min(a => a.OriginalReleaseDate!.Value.Year);
        int maxYear = _artist.SplitDate?.Year ?? DateTime.Now.Year;

        var grouped = albumsWithDate
            .GroupBy(a => a.OriginalReleaseDate!.Value.Year)
            .ToDictionary(g => g.Key, g => g.Count());

        _releaseCountPerYears = Enumerable
            .Range(minYear, maxYear - minYear + 1)
            .ToDictionary(year => year, year => grouped.TryGetValue(year, out var value) ? value : 0);
    }
    
    private void ToggleReleaseType(KeyValuePair<ReleaseType, bool> releaseType)
    {
        _availableReleaseTypes[releaseType.Key] = !releaseType.Value;
    }

    private async Task EditArtistShowModal()
    {
        if (null == _artist)
        {
            _logger.LogError("❌ Cannot edit artist: artist is null.");
            return;
        }

        await _artistEditModal.Show(_artist);
    }
    
    private async Task AddAlbumShowModal()
    {
        if (null == _artist)
        {
            _logger.LogError("❌ Cannot add album: artist is null.");
            return;
        }

        await _albumEditModal.Show(_artist);
    }

    private async Task OnQuitModal()
    {
        await LoadArtist();
    }
}