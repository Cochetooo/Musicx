using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Mappers;
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

    private MudTable<OutUserAlbumAttribute> _albumRatingsTable = null!;

    private OutGenericList<OutUserAlbumAttribute> _albumUserAttribs = new()
    {
        Items = [],
        Total = 0
    };
    
    private InUserAlbumAttribute _userAttribute = new();
 
    private bool _isArtworkRevealed;
    private bool _showDetailedView;

    private readonly List<BreadcrumbItem>? _breadcrumb = [];

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(AlbumView));
        _showDetailedView = !UserClientContext.CurrentUser?.PrefSimpleGenre ?? false;
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

        _userAttribute = new();
        
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
            
            _logger.LogInformation("✅ Previous and Next albums loaded.");
            await InvokeAsync(StateHasChanged);
        }

        _albumUserAttribs = await UcGetAlbumAttrs.ExecuteAsync(_album.Id);
        await _albumRatingsTable.ReloadServerData();

        // If a user is connected, we need to give a user_attribute object to the view
        if (UserClientContext.CurrentUser is not null)
        {
            // We try to find an existing attribute in the set of attributes
            var existingAttr = _albumUserAttribs
                .Items
                .FirstOrDefault(attr => attr.User.Id == UserClientContext.CurrentUser.Id);

            // If it exists, we give that to the view object.
            if (existingAttr is not null)
            {
                _userAttribute = existingAttr.ToRaw();
            }
            // If not, we just update the album and user ID to the already initialized object.
            else
            {
                _userAttribute.AlbumId = albumId;
                _userAttribute.UserId = UserClientContext.CurrentUser.Id;
            }
        }
        
        _logger.LogInformation("✅ User attributes loaded.");
        await InvokeAsync(StateHasChanged);
    }

    private async Task<TableData<OutUserAlbumAttribute>> LoadUserAttrData(TableState state, CancellationToken token)
    {
        if (_album is null)
        {
            _logger.LogWarning("⚠️ Album ID is null, cannot load user attributes data.");
            return new TableData<OutUserAlbumAttribute>
            {
                TotalItems = 0,
                Items = []
            };
        }
        
        var response = await UcGetAlbumAttrs.ExecuteAsync(
            _album.Id, 
            skip: state.Page * state.PageSize,
            take: state.PageSize,
            token
        );

        return new TableData<OutUserAlbumAttribute>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
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
    
    /**
     * Persistence
     */
    
    private async Task ChangeDateDiscovery(DateTime? dateValue)
    {
        if (UserClientContext.CurrentUser is null
            || !UserClientContext.Can("album.attr"))
        {
            _logger.LogInformation("❌ Could not change date of album.");
            return;
        }
        
        _userAttribute.DiscoveryDate = dateValue;
        await SaveUserAttr();
    }

    private async Task Rate(int? ratingValue)
    {
        if (UserClientContext.CurrentUser is null
            || !UserClientContext.Can("album.rate"))
        {
            _logger.LogInformation("❌ Could not rate album.");
            return;
        }
        
        _userAttribute.Rating = (short?)ratingValue;
        await SaveUserAttr();
    }

    private async Task SaveUserAttr()
    {
        if (_album is null)
        {
            _logger.LogInformation("❌ Could not save album.");
            return;
        }
        
        await UcSaveUserAttrib.ExecuteAsync(_userAttribute);
        
        // Refresh only album for new rating
        _album = await UcGet.ExecuteAsync(_album.Id, "artist_genre_stat");
        await InvokeAsync(StateHasChanged);
        
        _albumUserAttribs = await UcGetAlbumAttrs.ExecuteAsync(_album!.Id);
        _logger.LogInformation("✅ User attributes loaded.");
        await _albumRatingsTable.ReloadServerData();
        
        _logger.LogInformation("✅ Successfully saved user attributes.");
    }
}