using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Voting;

public partial class AlbumGenreVoteModal
{
    private ILogger _logger = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }
    
    private List<OutGenre> _availableGenres = [];
    private List<OutGenre> _primaryGenres = [];
    private List<OutGenre> _influenceGenres = [];

    private long _albumId;

    private string _selectedPrimaryText = string.Empty;
    private OutGenre? _selectedPrimaryGenre;
    private string _selectedInfluenceText = string.Empty;
    private OutGenre? _selectedInfluenceGenre;

    private MudDialog _modalRef = null!;

    protected override async Task OnInitializedAsync()
    {
        _logger = LoggerFactory.CreateLogger(nameof(AlbumGenreVoteModal));
        _availableGenres = await UcListGenres.ExecuteAsync(pagingOptions: new PagingOptions(100_000, 0));
    }

    public async Task Show(OutAlbum album)
    {
        _albumId = album.Id;
        await _modalRef.ShowAsync();
        
        _primaryGenres = album.PrimaryGenres?.Select(pg => pg.Genre)
            .ToList() ?? [];

        _influenceGenres = album.InfluenceGenres?.Select(ig => ig.Genre)
            .ToList() ?? [];

        await InvokeAsync(StateHasChanged);
    }

    private async Task Save()
    {
        _logger.LogInformation("💾 Saving user genres...");

        if (UserClientContext.CurrentUser is null)
        {
            _logger.LogInformation("❌ No user logged in, cannot save genres.");
            return;
        }

        await UcVoteAlbumGenre.ExecuteAsync(_primaryGenres.Select(pg => new InAlbumGenre
        {
            AlbumId = _albumId,
            TaggerId = UserClientContext.CurrentUser.Id,
            GenreId = pg.Id,
            Source = GenreVoteSource.User
        }).ToList());
        
        await UcVoteAlbumInfluence.ExecuteAsync(_influenceGenres.Select(pg => new InAlbumInfluence
        {
            AlbumId = _albumId,
            TaggerId = UserClientContext.CurrentUser.Id,
            GenreId = pg.Id,
            Source = GenreVoteSource.User
        }).ToList());

        Snackbar.Add("Genres saved successfully!", Severity.Success);
        
        Clean();

        await OnSave.InvokeAsync();
        await Hide();
    }

    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }

    private void Clean()
    {
        _primaryGenres.Clear();
        _influenceGenres.Clear();

        _selectedPrimaryGenre = null;
        _selectedPrimaryText = string.Empty;
        
        _selectedInfluenceGenre = null;
        _selectedInfluenceText = string.Empty;
    }

    private void AddPrimaryGenre()
    {
        if (_selectedPrimaryGenre is null)
        {
            return;
        }
        
        _primaryGenres.Add(_selectedPrimaryGenre);
        _selectedPrimaryGenre = null;
        _selectedPrimaryText = string.Empty;
    }

    private void AddInfluenceGenre()
    {
        if (_selectedInfluenceGenre is null)
        {
            return;
        }
        
        _influenceGenres.Add(_selectedInfluenceGenre);
        _selectedInfluenceGenre = null;
        _selectedInfluenceText = string.Empty;
    }
    
    private void OnDeletePrimaryGenre(OutGenre genre) => _primaryGenres.Remove(genre);
    private void OnDeleteInfluenceGenre(OutGenre genre) => _influenceGenres.Remove(genre);

    private async Task<IEnumerable<OutGenre>> SearchGenre(string? value, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return _availableGenres.Where(x => 
            x.CanonicalName
               .ToLower()
               .Contains(value, StringComparison.InvariantCultureIgnoreCase)
           && !_primaryGenres.Contains(x)
           && !_influenceGenres.Contains(x));
    }
}