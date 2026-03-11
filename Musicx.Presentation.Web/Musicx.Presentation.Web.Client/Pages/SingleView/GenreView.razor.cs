using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class GenreView
{
    [Parameter] public string? Id { get; set; }

    private sealed record GenreArtistCard(OutArtist Artist, decimal? Rating, long RatingsCount);

    private ILogger _logger = null!;

    private OutGenre? _genre;
    private OutGenericList<OutArtist> _artists = new();
    
    private long _albumCount;
    private decimal? _albumsAvgRating;
    private IReadOnlyList<OutUserYearlyRating> _yearlyRatings = [];
    private readonly List<ChartSeries<double>> _yearlyRatingsSeries = [];
    private readonly LineChartOptions _lineChartOptions = new()
    {
        InterpolationOption = InterpolationOption.NaturalSpline,
        LineDisplayType = LineDisplayType.Area,
        LineStrokeWidth = 4,
        ShowLegend = false,
        YAxisTicks = 10
    };
    
    private string[] _yearlyRatingsXAxis = [];
    private bool _useShortName;
    private MudTable<OutAlbum>? _albumTable;
    private ElementReference _topAlbumsScroller;

    private List<OutAlbum> _topAlbums = [];
    private List<GenreArtistCard> _topArtists = [];

    private bool _canScrollTopAlbumsLeft;
    private bool _canScrollTopAlbumsRight;

    private string DisplayedName => _useShortName &&
                                     !string.IsNullOrWhiteSpace(_genre?.ShortName)
        ? _genre.ShortName!
        : _genre?.CanonicalName ?? string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        await LoadGenre();
        
        if (_topAlbums.Count == 0)
        {
            _canScrollTopAlbumsLeft = false;
            _canScrollTopAlbumsRight = false;
            return;
        }

        await RefreshTopAlbumsArrows();
        await InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(GenreView));
    }

    private async Task LoadGenre()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Genre ID is null or empty.");
            return;
        }

        if (!long.TryParse(Id, out var genreId))
        {
            _logger.LogError("❌ Invalid Genre ID format: {Id}", Id);
            return;
        }

        _genre = await UcGet.ExecuteAsync(genreId, joins: new GenreJoinSpecification
        {
            IncludeParents = true,
            IncludeChildren = true
        });

        if (_genre is null)
        {
            _logger.LogError("❌ No genre found for ID: {Id}", Id);
            return;
        }
        
        await InvokeAsync(StateHasChanged);
        
        _artists = await UcArtistByGenre.ExecuteAsync(_genre.Id, pagingOptions: new PagingOptions(Skip: 0, Take: 10));

        await BuildTopArtistsAsync();
        await InvokeAsync(StateHasChanged);
        
        await BuildTopAlbumsAsync();
        await InvokeAsync(StateHasChanged);

        if (_albumTable is not null)
        {
            await _albumTable.ReloadServerData();
        }
        
        await LoadYearlyRatingsAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task BuildTopArtistsAsync()
    {
        _topArtists.Clear();

        foreach (var artist in _artists.Items)
        {
            var discography = await UcGetAlbums.ExecuteAsync(
                artist.Id,
                joins: new AlbumJoinSpecification { IncludeStats = true },
                order: new AlbumOrderSpecification { OriginalReleaseDate = -1 });

            var summary = RatingHelper.CalculateArtistRatingSummary(discography.Items);
            _topArtists.Add(new GenreArtistCard(artist, summary.Rating, summary.RatingsCount));
        }

        _topArtists = _topArtists
            .OrderByDescending(x => x.RatingsCount)
            .ThenByDescending(x => x.Rating ?? 0)
            .ToList();
    }

    private async Task BuildTopAlbumsAsync()
    {
        if (_genre is null)
        {
            return;
        }
        
        var response = await UcAlbumByGenre.ExecuteAsync(
            genreId: _genre.Id,
            genreOptions: GenreOptions.PrimaryGenre,
            pagingOptions: new PagingOptions(Take: 10, Skip: 0),
            order: new AlbumOrderSpecification { RatingCount = -1, RatingAverage = -2, Name = 3 },
            joins: new AlbumJoinSpecification
            {
                IncludeArtist = true,
                IncludePrimaryGenres = true,
                IncludeStats = true
            });

        _topAlbums = response.Items;
    }
    
    private async Task<TableData<OutAlbum>> LoadAlbumsData(TableState state, CancellationToken token)
    {
        if (_genre is null)
        {
            _logger.LogError("❌ No genre found for ID: {Id}", Id);
            return new TableData<OutAlbum>
            {
                TotalItems = 0,
                Items = []
            };;
        }
        
        var response = await UcAlbumByGenre.ExecuteAsync(
            genreId: _genre.Id,
            genreOptions: GenreOptions.PrimaryGenre,
            pagingOptions: new PagingOptions(Take: state.PageSize, Skip: state.Page * state.PageSize),
            order: new AlbumOrderSpecification
            {
                OriginalReleaseDate = 1,
                Name = 2,
            },
            joins: new AlbumJoinSpecification
            {
                IncludeArtist = true,
                IncludePrimaryGenres = true,
                IncludeInfluenceGenres = true,
                IncludeStats = true
            }
        );

        _albumCount = response.Total;

        return new TableData<OutAlbum>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }
    
    private async Task LoadYearlyRatingsAsync()
    {
        if (_genre is null || UserClientContext.CurrentUser is null)
        {
            _yearlyRatings = [];
            _yearlyRatingsSeries.Clear();
            _yearlyRatingsXAxis = [];
            return;
        }

        var yearlyRatings = await UcUserYearlyRatings.ExecuteAsync(
            bucketSize: 5,
            genreId: _genre.Id);
        
        _yearlyRatings = yearlyRatings ?? [];
        _albumsAvgRating = _yearlyRatings.Select(r => r.AverageRating).Average();

        _yearlyRatingsSeries.Clear();
        _yearlyRatingsSeries.Add(new ChartSeries<double>
        {
            Name = "Average rating",
            Data = _yearlyRatings.Select(r => (double)Math.Round(r.AverageRating / 500.0m, 2)).ToArray()
        });

        _yearlyRatingsXAxis = _yearlyRatings
            .Select(r => r.YearBucketStart.ToString())
            .ToArray();
    }

    private void ToggleName() => _useShortName = !_useShortName;
    
    private string FormatEra()
    {
        var start = _genre?.EraStart?.Year.ToString() ?? "?";
        var end = _genre?.EraEnd?.Year.ToString() ?? "?";
        return $"{start} – {end}";
    }

    private string GetHeaderStyle()
    {
        var background = _genre!.Color ?? ColorHelper.DarkColor;
        background += "66"; // Add transparency
        
        return $"background: linear-gradient(to right, {background} 0%, {background} 60%, transparent 100%); font-weight: bold;";
    }
    
    private async Task ScrollTopAlbumsAsync(string direction)
    {
        if (_topAlbums.Count == 0)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsync("genreView.scrollHorizontal", _topAlbumsScroller, direction, 360);
        await RefreshTopAlbumsArrows();
    }


    private Task ScrollTopAlbumsLeftAsync() => ScrollTopAlbumsAsync("left");
    private Task ScrollTopAlbumsRightAsync() => ScrollTopAlbumsAsync("right");

    private async Task RefreshTopAlbumsArrows()
    {
        var state = await JsRuntime.InvokeAsync<HorizontalScrollState>("genreView.getHorizontalState", _topAlbumsScroller);
        _canScrollTopAlbumsLeft = state.CanScrollLeft;
        _canScrollTopAlbumsRight = state.CanScrollRight;
        await InvokeAsync(StateHasChanged);
    }

    private sealed class HorizontalScrollState
    {
        public bool CanScrollLeft { get; set; }
        public bool CanScrollRight { get; set; }
    }
}