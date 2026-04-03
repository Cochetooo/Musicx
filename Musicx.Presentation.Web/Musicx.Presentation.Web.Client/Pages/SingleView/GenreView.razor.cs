using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;
using Musicx.Presentation.Web.Client.Components.Charts;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class GenreView : IAsyncDisposable
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private OutGenre? _genre;
    
    private long _albumCount;
    private decimal? _albumsAvgRating;
    private IReadOnlyList<OutUserYearlyRating> _yearlyRatings = [];
    private IReadOnlyList<D3LinePoint> _yearlyRatingsChartData = [];
    
    private bool _useShortName;
    private MudTable<OutAlbum>? _albumTable;
    private ElementReference _topAlbumsScroller;

    private List<OutAlbum> _topAlbums = [];
    private List<OutArtistAlbumSummary> _topArtists = [];

    private bool _canScrollTopAlbumsLeft;
    private bool _canScrollTopAlbumsRight;
    
    private IJSObjectReference? _genreViewModule;

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

        var dataView = await Api.GetDataViewAsync<OutGenreDataView>(
            "genres", 
            genreId, 
            new Dictionary<string, object?> {
                { "userId", UserClientContext.CurrentUser?.Id }
            }
        );

        if (dataView is null)
        {
            _logger.LogError("❌ No genre data view found for ID: {Id}", Id);
            return;
        }
        
        _genre = dataView.Genre;
        _topArtists = dataView.TopArtists.ToList();
        _topAlbums = dataView.TopAlbums.Items;
        _yearlyRatings = dataView.YearlyRatings;
        _albumsAvgRating = dataView.AlbumsAverageRating;
        _yearlyRatingsChartData = _yearlyRatings
            .Select(r => new D3LinePoint(r.YearBucketStart.ToString(), (double)Math.Round(r.AverageRating / 500.0m, 2)))
            .ToList();
        
        await InvokeAsync(StateHasChanged);

        if (_albumTable is not null)
        {
            await _albumTable.ReloadServerData();
        }
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
        
        var response = await Api.FindAlbumsByGenreAsync(
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

        _genreViewModule ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Js/genreView.js");
        await _genreViewModule.InvokeVoidAsync("scrollHorizontal", _topAlbumsScroller, direction, 360);
        
        await RefreshTopAlbumsArrows();
    }


    private Task ScrollTopAlbumsLeftAsync() => ScrollTopAlbumsAsync("left");
    private Task ScrollTopAlbumsRightAsync() => ScrollTopAlbumsAsync("right");

    private async Task RefreshTopAlbumsArrows()
    {
        _genreViewModule ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Js/genreView.js");
        var state = await _genreViewModule.InvokeAsync<HorizontalScrollState>("getHorizontalState", _topAlbumsScroller);
        
        _canScrollTopAlbumsLeft = state.CanScrollLeft;
        _canScrollTopAlbumsRight = state.CanScrollRight;
        await InvokeAsync(StateHasChanged);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_genreViewModule is not null)
        {
            await _genreViewModule.DisposeAsync();
        }
    }

    private sealed class HorizontalScrollState
    {
        public bool CanScrollLeft { get; set; }
        public bool CanScrollRight { get; set; }
    }
}