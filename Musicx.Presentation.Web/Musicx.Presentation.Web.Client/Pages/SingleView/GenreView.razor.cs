using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Options;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class GenreView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private OutGenre? _genre;
    private List<OutAlbum> _genreAlbums = [];

    private readonly List<BreadcrumbItem>? _breadcrumb = [];

    protected override async Task OnParametersSetAsync()
    {
        await LoadGenre();

        if (_breadcrumb is not null && _genre is not null)
        {
            _breadcrumb.Clear();
            _breadcrumb.Add(new("Musicx", href: "/"));
            _breadcrumb.Add(new("Genres", href: "#"));
            _breadcrumb.Add(new(_genre.Name, href: "#"));
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

        _genre = await UcGet.ExecuteAsync(genreId, "parents_children");

        if (_genre is null)
        {
            _logger.LogError("❌ No genre found for ID: {Id}", Id);
            return;
        }
        
        _logger.LogInformation($"✅ Genre loaded: {_genre.Name} ({_genre.Id})");
        await InvokeAsync(StateHasChanged);

        _genreAlbums = await UcAlbumByGenre.ExecuteAsync(
            genreId: _genre.Id,
            genreOptions: GenreOptions.PrimaryGenre,
            take: 100,
            query: "artist_genre"
        );
        
        _logger.LogInformation($"🏷️ Albums loaded : {_genreAlbums.Count}");
        await InvokeAsync(StateHasChanged);
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