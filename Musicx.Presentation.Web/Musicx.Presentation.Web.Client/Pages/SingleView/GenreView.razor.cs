using Microsoft.AspNetCore.Components;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(GenreView));

        await LoadGenre();
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

        _genre = await UcGet.ExecuteAsync(genreId, "children");

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
            take: 40,
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