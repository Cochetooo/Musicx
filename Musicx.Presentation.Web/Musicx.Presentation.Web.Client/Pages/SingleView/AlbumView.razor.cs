using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Song;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;
using Musicx.Presentation.Web.Client.Modals.Voting;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class AlbumView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private AlbumEditModal _albumEditModal = null!;
    private AlbumTrackListEditModal _trackListEditModal = null!;
    private AlbumGenreVoteModal _genreVoteModal = null!;

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

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(AlbumView));
        _showDetailedView = !UserClientContext.CurrentUser?.PrefSimpleGenre ?? false;
    }
    
    protected override async Task OnParametersSetAsync()
        => await LoadAlbum();

    private async Task LoadAlbum()
    {
        _logger.LogInformation("🔄️ Loading Album Data...");
        
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

        _album = await UcGet.ExecuteAsync(albumId, joins: new AlbumJoinSpecification
        {
            IncludeArtist = true,
            IncludePrimaryGenres = true,
            IncludeInfluenceGenres = true,
            IncludeStats = true
        });
        
        if (_album is null)
        {
            _logger.LogWarning("❌ No album found for ID: {Id}", Id);
            return;
        }

        _userAttribute = new();
        
        _logger.LogInformation($"✅ Album loaded: {_album.Name} ({_album.Id})");
        await InvokeAsync(StateHasChanged);

        _albumSongs = await UcFindSongs.ExecuteAsync(_album.Id, order: new SongOrderSpecification
        {
            TrackNumber = 1,
            Title = 2
        });
        _logger.LogInformation($"✅ Album Songs loaded: {_albumSongs.Count}");
        await InvokeAsync(StateHasChanged);

        _previousAlbum = null;
        _nextAlbum = null;

        if (_album.Artist is not null)
        {
            var response = await UcGetArtistAlbums.ExecuteAsync(_album.Artist.Id, order: new AlbumOrderSpecification
            {
                OriginalReleaseDate = 1,
                Name = 2
            });
            var artistAlbums = response.Items;

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

        // If a user is connected, we need to give a user_attribute object to the view
        if (UserClientContext.CurrentUser is not null)
        {
            var existingAttr = await UcFindUserServiceAlbumAttr.ExecuteAsync(
                UserClientContext.CurrentUser.Id,
                _album.Id);

            // If it exists, we give that to the view object.
            if (existingAttr is not null)
            {
                _userAttribute = existingAttr.ToRaw();
                _logger.LogInformation("ℹ️ User rating found.");
            }
            // If not, we just update the album and user ID to the already initialized object.
            else
            {
                _userAttribute.AlbumId = albumId;
                _userAttribute.UserId = UserClientContext.CurrentUser.Id;
                _logger.LogInformation("ℹ️ No user rating.");
            }
        }
        
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
        
        var response = await UcFindAlbumAttrs.ExecuteAsync(
            _album.Id, 
            order: new UserAlbumAttrOrderSpecification
            {
                CreatedAt = -1,
                UserName = 2,
                Rating = 3
            },
            pagingOptions: new PagingOptions(Take: state.PageSize, Skip: state.Page * state.PageSize),
            cancellationToken: token
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

    private async Task OpenVoteGenreModal()
    {
        if (_album is null)
        {
            _logger.LogError("❌ Cannot open vote genre modal: album is null.");
            return;
        }
        
        await _genreVoteModal.Show(_album);
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
        _album = await UcGet.ExecuteAsync(_album.Id, joins: new AlbumJoinSpecification
        {
            IncludeArtist = true,
            IncludePrimaryGenres = true,
            IncludeInfluenceGenres = true,
            IncludeStats = true
        });
        await InvokeAsync(StateHasChanged);
        
        _albumUserAttribs = await UcFindAlbumAttrs.ExecuteAsync(_album!.Id);
        _logger.LogInformation("✅ User attributes loaded.");
        await _albumRatingsTable.ReloadServerData();
        
        _logger.LogInformation("✅ Successfully saved user attributes.");
    }
}