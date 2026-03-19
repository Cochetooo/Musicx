using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
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
    private List<OutAlbum> _filteredAlbums = [];
    private OutGenericList<OutUserAlbumAttribute>? _userAttrs;
    private Dictionary<ReleaseType, bool> _availableReleaseTypes = [];
    private Dictionary<int, int> _releaseCountPerYears = [];
    private OutArtistRatingStat _artistRatingSummary = new();
    private bool _isCurrentUserFollowing;
    private long _followersCount;
    
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
        
        var dataView = await UcArtistDataView.ExecuteAsync(artistId, UserClientContext.CurrentUser?.Id);

        if (dataView is null)
        {
            _logger.LogError("⚠️ No artist found for ID: {Id}", Id);
            return;
        }

        _artist = dataView.Artist;
        _artistAlbums = dataView.Albums;
        _artistRatingSummary = dataView.Artist.Stats;
        _userAttrs = dataView.UserAttributes;
        _isCurrentUserFollowing = dataView.IsCurrentUserFollowing;
        _followersCount = dataView.FollowersCount;

        _logger.LogInformation($"✅ Artist loaded: {_artist.Name} ({_artist.Id})");
        
        _filteredAlbums = new List<OutAlbum>(_artistAlbums.Items);
        
        _availableReleaseTypes = _artistAlbums
            .Items
            .Where(s => s.ReleaseType.HasValue)
            .Select(s => s.ReleaseType!.Value)
            .Distinct()
            .ToDictionary(r => r, r => r is ReleaseType.Lp or ReleaseType.MixTape or ReleaseType.Soundtrack);
        
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
            await UcSaveArtistAttr.ExecuteAsync(new InUserArtistAttribute
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