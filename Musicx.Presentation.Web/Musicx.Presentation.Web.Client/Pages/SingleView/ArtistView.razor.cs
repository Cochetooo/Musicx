using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;
using Musicx.Domain.Enums;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class ArtistView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private AlbumEditModal? _albumEditModal;
    private LineChart<int> _historyChart = null!;
    
    private OutArtist? _artist;
    private List<OutAlbum> _artistAlbums = [];
    private Dictionary<ReleaseType, bool> _availableReleaseTypes = [];
    private Dictionary<int, int> _releaseCountPerYears = [];
    private bool _isReleasesListView;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(ArtistView));

        await LoadArtist();
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
        
        _artistAlbums = await UcGetAlbums.ExecuteAsync(_artist.Id);
        _logger.LogInformation("🎵 Retrieved {Count} albums for artist {ArtistId}", _artistAlbums.Count, _artist.Id);
        
        _availableReleaseTypes = _artistAlbums
            .Where(s => s.ReleaseType.HasValue)
            .Select(s => s.ReleaseType!.Value)
            .Distinct()
            .ToDictionary(r => r, r => r is ReleaseType.Lp or ReleaseType.MixTape);
        
        await InvokeAsync(StateHasChanged);
     
        CalculateReleasesPerYear();
        await UpdateChartAsync();
        
        _logger.LogInformation($"✅ Chart created.");
    }
    
    private async Task UpdateChartAsync()
    {
        await _historyChart.Clear();

        var labels = _releaseCountPerYears.Keys.Select(y => y.ToString()).ToList();
        var dataset = new LineChartDataset<int>
        {
            Label = "# of releases",
            Data = _releaseCountPerYears
                .Select(g => g.Value)
                .ToList(),
            Fill = true,
            PointRadius = 2,
            CubicInterpolationMode = "monotone"
        };
        
        await _historyChart.AddLabelsDatasetsAndUpdate(labels, dataset);
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
                ReleaseType: ReleaseType.Lp or ReleaseType.Ep or ReleaseType.MixTape
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

    private void AddAlbumShowModal()
    {
        if (null == _artist)
        {
            _logger.LogError("❌ Cannot add album: artist is null.");
            return;
        }

        _albumEditModal?.Show(_artist);
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