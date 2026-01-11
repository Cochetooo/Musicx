using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class GenreView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private OutGenre? _genre;

    private readonly List<BreadcrumbItem>? _breadcrumb = [];
    private long _albumCount;
    private decimal? _albumsAvgRating;
    
    private MudTable<OutAlbum>? _albumTable;

    protected override async Task OnParametersSetAsync()
    {
        await LoadGenre();

        if (_breadcrumb is not null && _genre is not null)
        {
            _breadcrumb.Clear();
            _breadcrumb.Add(new("Musicx", href: "/"));
            _breadcrumb.Add(new("Genres", href: "#"));
            _breadcrumb.Add(new(_genre.CanonicalName, href: "#"));
        }
        
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
        
        _logger.LogInformation($"✅ Genre loaded: {_genre.CanonicalName} ({_genre.Id})");

        if (_albumTable is not null)
        {
            await _albumTable.ReloadServerData();
        }
        
        await InvokeAsync(StateHasChanged);
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
            joins: new AlbumJoinSpecification
            {
                IncludeArtist = true,
                IncludePrimaryGenres = true,
                IncludeInfluenceGenres = true,
                IncludeStats = true
            }
        );

        _albumCount = response.Total;
        _albumsAvgRating = response.AverageRating;
        await InvokeAsync(StateHasChanged);

        return new TableData<OutAlbum>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }

    private string GetHeaderStyle()
    {
        var background = _genre!.Color ?? ColorHelper.DarkColor;
        var textColor = ColorHelper.IsColorLight(background)
            ? ColorHelper.DarkColor
            : "#ffffff";
        
        return $"background-color: {background}; color: {textColor}; font-weight: bold;";
    }
}