using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class AlbumView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private AlbumEditModal? _albumEditModal;
    private AlbumTrackListEditModal? _trackListEditModal;

    private OutAlbum? _album;
    private List<OutSong> _albumSongs = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(AlbumView));

        await LoadAlbum();
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

        if (_album.PrimaryGenres is not null && 0 < _album.PrimaryGenres.Count)
        {
            _album.PrimaryGenres = await UcFindInGenres.ExecuteAsync(_album.PrimaryGenres.Select(g => g.Id).ToList());
            _logger.LogInformation($"🏷️ Primary genres loaded for {_album.Name} ({_album.Id})");
            await InvokeAsync(StateHasChanged);
        }

        if (_album.InfluenceGenres is not null && 0 < _album.InfluenceGenres.Count)
        {
            _album.InfluenceGenres = await UcFindInGenres.ExecuteAsync(_album.InfluenceGenres.Select(g => g.Id).ToList());
            _logger.LogInformation($"🏷️ Influence genres loaded for {_album.Name} ({_album.Id})");
            await InvokeAsync(StateHasChanged);
        }
    }

    private void EditTrackListShowModal()
    {
        if (null == _album)
        {
            _logger.LogError("❌ Cannot edit tracklist: album is null.");
            return;
        }
        
        _trackListEditModal?.Show(_album);
    }

    private void EditAlbumShowModal()
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
        
        _albumEditModal?.Show(_album.Artist, _album);
    }

    private async Task OnQuitModal()
    {
        await LoadAlbum();
    }
}