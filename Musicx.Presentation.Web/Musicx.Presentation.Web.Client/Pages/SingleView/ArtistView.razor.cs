using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;
using Musicx.Presentation.Web.Client.Modals.Admin.Artists;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class ArtistView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private ArtistEditModal _artistEditModal = null!;
    private AlbumEditModal _albumEditModal = null!;
    
    private OutArtist? _artist;
    private List<OutAlbum> _artistAlbums = [];
    private Dictionary<ReleaseType, bool> _availableReleaseTypes = [];
    private Dictionary<int, int> _releaseCountPerYears = [];
    
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
    
    private bool _isReleasesListView;
    
    private readonly List<BreadcrumbItem>? _breadcrumb = 
    [
        new("Musicx", href: "/"),
    ];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(ArtistView));

        await LoadArtist();

        if (_breadcrumb is not null && _artist is not null)
        {
            _breadcrumb.Add(new(_artist.Name, href: "#"));
        }

        await InvokeAsync(StateHasChanged);
    }

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
        
        _artistAlbums = await UcGetAlbums.ExecuteAsync(_artist.Id, "genre");
        _logger.LogInformation("🎵 Retrieved {Count} albums for artist {ArtistId}", _artistAlbums.Count, _artist.Id);
        
        _availableReleaseTypes = _artistAlbums
            .Where(s => s.ReleaseType.HasValue)
            .Select(s => s.ReleaseType!.Value)
            .Distinct()
            .ToDictionary(r => r, r => r is ReleaseType.Lp or ReleaseType.MixTape or ReleaseType.Soundtrack);
        
        await InvokeAsync(StateHasChanged);
     
        CalculateReleasesPerYear();
        UpdateChart();
        
        await InvokeAsync(StateHasChanged);
        
        _logger.LogInformation($"✅ Chart created.");
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

    private void ToggleListView(bool newValue)
    {
        _isReleasesListView = newValue;
    }
    
    private void ToggleReleaseType(KeyValuePair<ReleaseType, bool> releaseType)
    {
        _availableReleaseTypes[releaseType.Key] = !releaseType.Value;
    }

    private Variant GetVariant(KeyValuePair<ReleaseType, bool> releaseType)
        => _availableReleaseTypes[releaseType.Key]
            ? Variant.Filled
            : Variant.Text;
    
    private Variant GetListVariant(bool inverted = false)
        => _isReleasesListView
            ? inverted ? Variant.Text : Variant.Filled
            : inverted ? Variant.Filled : Variant.Text;

    private async Task EditArtistShowModal()
    {
        if (null == _artist)
        {
            _logger.LogError("❌ Cannot add artist: artist is null.");
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

    private string GetSimplifiedGenreStyle(OutAlbum album)
        =>
            $"background: {album.SimplifiedGenreColor}; color: {(ColorHelper.IsColorLight(album.SimplifiedGenreColor!) 
                ? ColorHelper.DarkColor 
                : "white")};";
}