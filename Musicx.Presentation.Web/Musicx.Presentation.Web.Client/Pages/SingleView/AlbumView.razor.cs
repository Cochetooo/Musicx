using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Requests;
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
    private OutAlbum? _previousAlbum, _nextAlbum;

    private bool _isArtworkRevealed;
    private bool _showDetailedView;

    private readonly List<BreadcrumbItem>? _breadcrumb = [];

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(AlbumView));
    }
    
    protected override async Task OnParametersSetAsync()
    {
        await LoadAlbum();

        if (_breadcrumb is not null && _album is not null)
        {
            _breadcrumb.Clear();
            _breadcrumb.Add(new("Musicx", href: "/"));
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

        _album = await UcGet.ExecuteAsync(albumId, "artist_genre_stat");
        
        if (_album is null)
        {
            _logger.LogWarning("❌ No album found for ID: {Id}", Id);
            return;
        }
        
        _logger.LogInformation($"✅ Album loaded: {_album.Name} ({_album.Id})");
        await InvokeAsync(StateHasChanged);

        _albumSongs = await UcGetSongs.ExecuteAsync(_album.Id);
        _logger.LogInformation($"✅ Album Songs loaded: {_albumSongs.Count}");
        await InvokeAsync(StateHasChanged);

        _previousAlbum = null;
        _nextAlbum = null;

        if (_album.Artist is not null)
        {
            var artistAlbums = await UcGetArtistAlbums.ExecuteAsync(_album.Artist.Id);

            var currentIndex = artistAlbums.FindIndex(a => a.Id == _album.Id);

            if (currentIndex != -1)
            {
                if (currentIndex > 0)
                {
                    _previousAlbum = artistAlbums[currentIndex - 1];
                }

                if (currentIndex < artistAlbums.Count - 1)
                {
                    _nextAlbum = artistAlbums[currentIndex + 1];
                }
            }
            
            _logger.LogInformation($"✅ Previous and Next albums loaded.");
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task EditTrackListShowModal()
    {
        if (null == _album)
        {
            _logger.LogError("❌ Cannot edit tracklist: album is null.");
            return;
        }

        if (_albumSongs.Count > 0)
        {
            await _trackListEditModal.Show(_album, _albumSongs);
        }
        else
        {
            await _trackListEditModal.Show(_album);
        }
        
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

    private async Task OnQuitModal()
    {
        await LoadAlbum();
    }

    private async Task Rate(int? ratingValue)
    {
        if (_album is null
            || UserClientContext.CurrentUser is null
            || !UserClientContext.Can("album.rate"))
        {
            _logger.LogInformation($"❌ Could not rate album.");
            return;
        }

        await UcSaveUserAttrib.ExecuteAsync(new InUserAlbumAttribute
        {
            UserId = UserClientContext.CurrentUser.Id,
            AlbumId = _album.Id,
            CollectionType = null,
            Review = null,
            Rating = (short?)ratingValue
        });
        
        _logger.LogInformation($"✅ Successfully saved new rating.");
    }
}