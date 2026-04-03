using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Contracts.Enums;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;
using Musicx.Presentation.Web.Client.Modals.Admin.Artists;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public enum ReleasesViewMode
{
    List,
    Grid,
    Timeline
}

public partial class ArtistView
{
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private ArtistEditModal _artistEditModal = null!;
    private AlbumEditModal _albumEditModal = null!;
    
    private OutArtist? _artist;
    private OutAlbumList _artistAlbums = new();
    private IReadOnlyList<OutArtistGenreStat> _primaryGenres = [];
    private IReadOnlyList<OutArtistGenreStat> _influences = [];
    private IReadOnlyList<OutArtistGenreStat> _descriptors = [];
    private IReadOnlyList<OutArtistGenreStat> _scenes = [];
    private IReadOnlyList<OutArtistGenreStat> _movements = [];
    private List<OutAlbum> _filteredAlbums = [];
    private OutGenericList<OutUserAlbumAttribute>? _userAttrs;
    private OutArtistRatingStat _artistRatingSummary = new();
    private bool _isCurrentUserFollowing;
    private long _followersCount;
    private bool _simpleGenreMode;
    
    private ReleasesViewMode _viewMode = ReleasesViewMode.List;
    private bool _groupByType = true;
    private double _zoomLevel = 1.2;
    private (string sortBy, bool asc) _selectedSort = ("ReleaseDate", false);

    protected override async Task OnParametersSetAsync()
        => await LoadArtist();

    protected override void OnInitialized()
    => _logger = LoggerProvider.CreateLogger(nameof(ArtistView));

    private async Task LoadArtist()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Artist ID is null or empty.");
            return;
        }

        if (!long.TryParse(Id, out var artistId))
        {
            _logger.LogError("❌ Invalid Artist ID format: {Id}", Id);
            return;
        }
        
        var dataView = await Api.GetDataViewAsync<OutArtistDataView>(
            "artists", 
            artistId, 
            new Dictionary<string, object?> {
                { "userId", UserClientContext.CurrentUser?.Id }
            }
        );

        if (dataView is null)
        {
            _logger.LogError("⚠️ No artist found for ID: {Id}", Id);
            return;
        }

        _artist = dataView.Artist;
        _artistAlbums = dataView.Albums;
        
        if (dataView.Artist.Stats is not null)
        {
            _artistRatingSummary = dataView.Artist.Stats;
        }
        
        _userAttrs = dataView.UserAttributes;
        _isCurrentUserFollowing = dataView.IsCurrentUserFollowing;
        _followersCount = dataView.FollowersCount;
        _primaryGenres = dataView.PrimaryGenres;
        _influences = dataView.Influences;
        _descriptors = dataView.Descriptors;
        _scenes = dataView.Scenes;
        _movements = dataView.Movements;
        _simpleGenreMode = UserClientContext.CurrentUser?.PrefSimpleGenre ?? false;

        _logger.LogInformation($"✅ Artist loaded: {_artist.Name} ({_artist.Id})");
        
        _filteredAlbums = new List<OutAlbum>(_artistAlbums.Items);
        
        await InvokeAsync(StateHasChanged);
        
        _logger.LogInformation($"✅ Chart created.");
    }

    private void OnSearch(string text)
    {
        _filteredAlbums = _artistAlbums.Items
            .Where(a => a.Name.ToLower().Contains(text.ToLower()))
            .ToList();
    }

    private void OnSort((string key, bool descending) sort)
    {
        if (sort.descending)
        {
            switch (sort.key)
            {
                case "Name":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.Name).ToList();
                    break;
                case "NbRating":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.Stats?.Count).ToList();
                    break;
                case "GlobalRating":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.Stats?.Average).ToList();
                    break;
                case "MyRating":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => _userAttrs?
                        .Items
                        .FirstOrDefault(i => i.Album.Id == a.Id)
                        ?.Rating).ToList();
                    break;
                case "ReleaseDate":
                    _filteredAlbums = _filteredAlbums.OrderByDescending(a => a.OriginalReleaseDate).ToList();
                    break;
            }
        }
        else
        {
            switch (sort.key)
            {
                case "Name":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.Name).ToList();
                    break;
                case "NbRating":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.Stats?.Count).ToList();
                    break;
                case "GlobalRating":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.Stats?.Average).ToList();
                    break;
                case "MyRating":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => _userAttrs?
                        .Items
                        .FirstOrDefault(i => i.Album.Id == a.Id)
                        ?.Rating).ToList();
                    break;
                case "ReleaseDate":
                    _filteredAlbums = _filteredAlbums.OrderBy(a => a.OriginalReleaseDate).ToList();
                    break;
            }
        }

        _selectedSort = sort;
        StateHasChanged();
    }
    
    private void OnViewModeChanged(ReleasesViewMode vm) => _viewMode = vm;
    private void OnZoomChanged(double zoom) => _zoomLevel = zoom;
    private void OnToggleGroup(bool toggle) => _groupByType = toggle;

    private void OnRatingModeChanged(RatingMode mode)
    {
        if (UserClientContext.CurrentUser is not null)
        {
            UserClientContext.CurrentUser.PrefRatingMode = mode;
            // @TODO Persist change
        }
    }
    
    private async Task ToggleArtistFollowAsync()
    {
        if (_artist is null || UserClientContext.CurrentUser is null || !UserClientContext.Can("user.album.attrs.save"))
        {
            return;
        }

        if (_isCurrentUserFollowing)
        {
            await Http.DeleteAsync($"/api/user-artist-attrs/{UserClientContext.CurrentUser.Id}/{_artist.Id}");
        }
        else
        {
            await Api.SaveAsync(new InUserArtistAttribute
            {
                UserId = UserClientContext.CurrentUser.Id,
                ArtistId = _artist.Id,
                Follow = true
            });
        }

        await LoadArtist();
    }

    private async Task EditArtistShowModal()
    {
        if (null == _artist)
        {
            _logger.LogError("❌ Cannot edit artist: artist is null.");
            return;
        }

        await _artistEditModal.Show(_artist);
    }
    
    private async Task AddAlbumShowModal()
    {
        if (null == _artist)
        {
            _logger.LogError("❌ Cannot add album: artist is null.");
            return;
        }

        await _albumEditModal.Show(_artist);
    }

    private async Task OnQuitModal()
    {
        await LoadArtist();
    }
}