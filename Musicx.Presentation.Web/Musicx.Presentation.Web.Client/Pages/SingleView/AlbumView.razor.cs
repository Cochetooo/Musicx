using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Albums;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Song;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Presentation.Web.Client.Modals.Admin.Albums;
using Musicx.Presentation.Web.Client.Modals.Voting;

namespace Musicx.Presentation.Web.Client.Pages.SingleView;

public partial class AlbumView
{
    private sealed record RatingFactor(
        string Label,
        Func<InUserAlbumAttribute, short?> Getter,
        Action<InUserAlbumAttribute, short?> Setter);
    
    [Parameter] public string? Id { get; set; }

    private ILogger _logger = null!;

    private AlbumEditModal _albumEditModal = null!;
    private AlbumTrackListEditModal _trackListEditModal = null!;
    private AlbumGenreVoteModal _genreVoteModal = null!;
    private AlbumSongGenreVoteModal _songGenreVoteModal = null!;

    private OutAlbum? _album;
    private List<OutSong> _albumSongs = [];
    private OutAlbum? _previousAlbum, _nextAlbum;

    private MudTable<OutUserAlbumAttribute> _albumRatingsTable = null!;
    
    private InUserAlbumAttribute _userAttribute = new();
    private IReadOnlyList<OutUserSongAttribute> _userSongAttributes = [];
    private bool _autoComputeAlbumRating;
    private bool _hasAnyAlbumFactor;
    private Dictionary<string, short> _albumFactorAverages = [];

    private readonly List<RatingFactor> _albumFactors =
    [
        new("Production", x => x.ProductionRating, (x, v) => x.ProductionRating = v),
        new("Lyrics", x => x.LyricsRating, (x, v) => x.LyricsRating = v),
        new("Instrumentation", x => x.InstrumentationRating, (x, v) => x.InstrumentationRating = v),
        new("Vocals", x => x.VocalsRating, (x, v) => x.VocalsRating = v),
        new("Atmosphere", x => x.AtmosphereRating, (x, v) => x.AtmosphereRating = v),
        new("Originality", x => x.OriginalityRating, (x, v) => x.OriginalityRating = v)
    ];
 
    private bool _isArtworkRevealed;
    private bool _showDetailedView;

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(AlbumView));
        _showDetailedView = !UserClientContext.CurrentUser?.PrefSimpleGenre ?? false;
        _autoComputeAlbumRating = UserClientContext.CurrentUser?.PrefAutoComputeAdvancedRatings ?? false;
    }
    
    protected override async Task OnParametersSetAsync()
        => await LoadAlbum();

    private async Task LoadAlbum()
    {
        _logger.LogInformation("🔄️ Loading Album Data...");

        if (!TryGetAlbumId(out var albumId))
        {
            Snackbar.Add(T["Web.AlbumView.InvalidAlbumIdFormat"], Severity.Warning);
            _logger.LogError("❌ Invalid Album ID format: {Id}", Id);
            return;
        }

        var dataView = await Api.GetDataViewAsync<OutAlbumDataView>(
            "albums", 
            albumId, 
            new Dictionary<string, object?> {
                { "userId", UserClientContext.CurrentUser?.Id }
            }
        );

        if (dataView is null)
        {
            _logger.LogWarning("❌ No album data view found for ID: {Id}", Id);
            Snackbar.Add(T["Web.AlbumView.NoAlbumFound"], Severity.Warning);
            return;
        }

        _album = dataView.Album;
        _albumSongs = dataView.Songs.ToList();
        _previousAlbum = dataView.PreviousAlbum;
        _nextAlbum = dataView.NextAlbum;
        _userAttribute = dataView.CurrentUserAttribute?.ToRaw() ?? new InUserAlbumAttribute
        {
            AlbumId = albumId,
            UserId = UserClientContext.CurrentUser?.Id ?? 0
        };
        _userSongAttributes = dataView.CurrentUserSongAttributes;
        RefreshAlbumFactorView(dataView.Ratings?.Items ?? []);

        _logger.LogInformation("✅ Album loaded from DataView: {Name} ({Id})", _album.Name, _album.Id);
        await InvokeAsync(StateHasChanged);
    }

    private bool TryGetAlbumId(out long albumId)
    {
        albumId = 0;

        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Album ID is null or empty.");
            return false;
        }

        if (!long.TryParse(Id, out albumId))
        {
            _logger.LogError("❌ Invalid Album ID format: {Id}", Id);
            return false;
        }

        return true;
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
        if (_album is null)
        {
            _logger.LogError("❌ Cannot edit tracklist: album is null.");
            Snackbar.Add(T["Web.AlbumView.CannotEditTracklistAlbumNull"], Severity.Warning);
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
        if (_album?.Artist is null)
        {
            _logger.LogError("⚠️ Cannot edit album: album/artist is null.");
            Snackbar.Add(T["Web.AlbumView.CannotEditAlbumNull"], Severity.Warning);
            return;
        }
        
        await _albumEditModal.Show(_album.Artist, _album);
    }

    private async Task OnQuitModal() => await LoadAlbum();
    
    private async Task ChangeDateDiscovery(DateTime? dateValue)
    {
        if (UserClientContext.CurrentUser is null
            || !UserClientContext.Can("album.attr"))
        {
            _logger.LogInformation("❌ Could not change date of album.");
            Snackbar.Add(T["Web.AlbumView.NotAllowedChangeDate"], Severity.Warning);
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
            Snackbar.Add(T["Web.AlbumView.CannotOpenVoteAlbumNull"], Severity.Warning);
            return;
        }
        
        await _genreVoteModal.Show(_album);
    }
    
    private async Task OpenSongVoteModal()
    {
        if (_album is null)
        {
            return;
        }

        await _songGenreVoteModal.Show(_albumSongs, _userSongAttributes);
    }

    private async Task Rate(int? ratingValue)
    {
        if (UserClientContext.CurrentUser is null
            || !UserClientContext.Can("album.rate"))
        {
            _logger.LogInformation("❌ Could not rate album.");
            Snackbar.Add(T["Web.AlbumView.CannotRateAlbumNull"], Severity.Warning);
            return;
        }
        
        _userAttribute.Rating = (short?)ratingValue;
        await SaveUserAttr();
    }
    
    private async Task OnAlbumFactorChanged(RatingFactor factor, short? value)
    {
        factor.Setter(_userAttribute, value);
        _hasAnyAlbumFactor = GetAlbumFactorValues().Count > 0;

        if (_autoComputeAlbumRating && HasAllAlbumFactors())
        {
            await ComputeAlbumRatingFromFactors();
            return;
        }

        await SaveUserAttr();
    }

    private async Task OnAutoComputeAlbumRatingChanged(bool value)
    {
        _autoComputeAlbumRating = value;
        if (_autoComputeAlbumRating && HasAllAlbumFactors())
        {
            await ComputeAlbumRatingFromFactors();
            return;
        }

        await SaveUserAttr();
    }

    private async Task ComputeAlbumRatingFromFactors()
    {
        var values = GetAlbumFactorValues();
        if (values.Count == 0)
        {
            return;
        }

        _userAttribute.Rating = (short)Math.Round(values.Select(v => (int)v).Average());
        await SaveUserAttr();
    }

    private List<short> GetAlbumFactorValues()
        => _albumFactors
            .Select(x => x.Getter(_userAttribute))
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();

    private bool HasAllAlbumFactors()
        => _albumFactors.All(x => x.Getter(_userAttribute).HasValue);

    private void RefreshAlbumFactorView(IReadOnlyCollection<OutUserAlbumAttribute> ratings)
    {
        _hasAnyAlbumFactor = GetAlbumFactorValues().Count > 0;
        _albumFactorAverages = new Dictionary<string, short>();

        var lookups = new Dictionary<string, IEnumerable<short?>>()
        {
            ["Production"] = ratings.Select(x => x.ProductionRating),
            ["Lyrics"] = ratings.Select(x => x.LyricsRating),
            ["Instrumentation"] = ratings.Select(x => x.InstrumentationRating),
            ["Vocals"] = ratings.Select(x => x.VocalsRating),
            ["Atmosphere"] = ratings.Select(x => x.AtmosphereRating),
            ["Originality"] = ratings.Select(x => x.OriginalityRating)
        };

        foreach (var (name, values) in lookups)
        {
            var rated = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
            if (rated.Count == 0)
            {
                continue;
            }

            _albumFactorAverages[name] = (short)Math.Round(rated.Select(v => (int)v).Average());
        }
    }

    private async Task SaveUserAttr()
    {
        if (_album is null)
        {
            _logger.LogInformation("❌ Could not save album.");
            return;
        }
        
        await Api.SaveAsync(_userAttribute);
        await _albumRatingsTable.ReloadServerData();

        Snackbar.Add(T["Web.AlbumView.RatingSaved"], Severity.Success);
        await LoadAlbum();
    }
}