using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class AlbumView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private AlbumEditModal _albumEditModal = null!;
    private AlbumTrackListEditModal _trackListEditModal = null!;

    private OutAlbum? _album;
    private List<OutSong> _albumSongs = [];

    private bool _showDetailedView;

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

        _logger = LoggerProvider.CreateLogger(nameof(AlbumView));

        await LoadAlbum();

        if (_breadcrumb is not null && _album is not null)
        {
            _breadcrumb.Add(new(_album.Artist?.Name ?? "?",
                href: _album is { Artist: not null } ? "/Artist/" + _album.Artist.Id : "#"));
            
            _breadcrumb.Add(new(_album.Name, href: "#"));
        }
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadAlbum()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Album ID is null or empty.");
            return;
        }
        
        if (!long.TryParse(Id, out var albumId))
        {
            _logger.LogError("❌ Invalid Album ID format: {Id}", Id);
            return;
        }

        _album = await UcGet.ExecuteAsync(albumId, "artist_genre");
        
        if (_album is null)
        {
            _logger.LogWarning("❌ No album found for ID: {Id}", Id);
            return;
        }
        
        _logger.LogInformation($"✅ Album loaded: {_album.Name} ({_album.Id})");
        await InvokeAsync(StateHasChanged);
    }

    private async Task EditTrackListShowModal()
    {
        if (null == _album)
        {
            _logger.LogError("❌ Cannot edit tracklist: album is null.");
            return;
        }
        
        await _trackListEditModal.Show(_album);
    }

    private async Task EditAlbumShowModal()
    {
        if (null == _album)
        {
            _logger.LogError("❌ Cannot edit album: album is null.");
            return;
        }
        
        if (null == _album.Artist)
        {
            _logger.LogError("⚠️ Cannot edit album: artist is null.");
            return;
        }
        
        await _albumEditModal.Show(_album.Artist, _album);
    }
    
    private string GetSimplifiedGenreStyle(OutAlbum album)
        =>
            $"background: {album.SimplifiedGenreColor}; color: {(ColorHelper.IsColorLight(album.SimplifiedGenreColor!) 
                ? ColorHelper.DarkColor 
                : "white")}; font-weight: bold";

    private async Task OnQuitModal()
    {
        await LoadAlbum();
    }
}