using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Albums;

public partial class AlbumEditModal
{
    private ILogger _logger = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private OutArtist? _artist;
    private InAlbum _album = null!;
    
    private IList<OutGenre> _availableGenres = [];
    private IList<OutGenre> _primaryGenres = [];
    private IList<OutGenre> _influenceGenres = [];

    private string _selectedPrimaryText = string.Empty;
    private string _selectedInfluenceText = string.Empty;
    private OutGenre? _selectedPrimaryGenre;
    private OutGenre? _selectedInfluenceGenre;

    private DateRange? _recordingDates;

    private bool _isLoading;

    private MudTextField<string> _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private MudDialog _modalRef = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        
        _logger = LoggerFactory.CreateLogger(nameof(AlbumEditModal));

        _album = new InAlbum();
        _availableGenres = await UcListGenres.ExecuteAsync(take: 10_000);
    }

    private async Task Save()
    {
        _album.PrimaryGenreIds = _primaryGenres.Select(g => g.Id).ToList();
        _album.InfluenceGenreIds = _influenceGenres.Select(g => g.Id).ToList();
        _album.BeginRecordDate = _recordingDates?.Start;
        _album.EndRecordDate = _recordingDates?.End;
        
        _logger.LogDebug($"⛏️ AlbumEditModal : Persisting primary genres {string.Join(",", _album.PrimaryGenreIds)} " +
                               $"and influences {string.Join(",", _album.InfluenceGenreIds)}");
        
        await UcSave.ExecuteAsync(_album);
        await OnSave.InvokeAsync();

        Clean();
        
        await Hide();
    }

    public async Task Show(OutArtist artist, OutAlbum? album = null)
    {
        _artist = artist;
        _album.ArtistId = _artist.Id;
        
        await _modalRef.ShowAsync();
        
        if (null != album)
        {
            _album = album.ToRaw();
            
            _primaryGenres = album.PrimaryGenres?.ToList() ?? [];
            _influenceGenres = album.InfluenceGenres?.ToList() ?? [];
            
            _recordingDates = new DateRange(
                _album.BeginRecordDate,
                _album.EndRecordDate
            );
            
            await InvokeAsync(StateHasChanged);
            
            await _nameTextEdit.SetText(album.Name);
        }
    }
    
    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }

    private async Task UpdateArtwork()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);
        _artworkCts?.Cancel();
        _artworkCts?.Dispose();

        _artworkCts = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(_album.Name))
        {
            _isLoading = false;
            return;
        }

        try
        {
            await Task.Delay(2000, _artworkCts.Token);

            var content = await Http.GetStringAsync(
                $"api/external/album?name={Uri.EscapeDataString(_album.Name)}&artist={Uri.EscapeDataString(_artist!.Name)}",
                _artworkCts.Token
            );

            var response = JsonSerializer.Deserialize<OutAlbum?>(content, JsonHelper.OptionsDefault);
            
            if (null == response)
            {
                _logger.LogWarning($"⚠️ Album not found: {_album.Name}");
                _isLoading = false;
                return;
            }
            
            _logger.LogInformation("ℹ️ Retrieved Artwork Url : " + response.ArtworkUrl);
            _album.ArtworkUrl = response.ArtworkUrl;
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (TaskCanceledException)
        {

        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Error while retrieving artwork : " + ex.Message);
            _isLoading = false;
        }
    }
    
    private async Task NameTextChanged(string newValue)
    {
        _album.Name = newValue;

        await UpdateArtwork();
    }

    private void OnDeletePrimaryGenre(OutGenre genre)
    {
        _primaryGenres.Remove(genre);
    }

    private void AddPrimaryGenre()
    {
        if (_selectedPrimaryGenre is null)
        {
            return;
        }
        
        _primaryGenres.Add(_selectedPrimaryGenre);
        _selectedPrimaryGenre = null;
        _selectedPrimaryText = "";
    }
    
    private void OnDeleteInfluenceGenre(OutGenre genre)
    {
        _influenceGenres.Remove(genre);
    }

    private void AddInfluenceGenre()
    {
        if (_selectedInfluenceGenre is null)
        {
            return;
        }
        
        _influenceGenres.Add(_selectedInfluenceGenre);
        _selectedInfluenceGenre = null;
        _selectedInfluenceText = "";
    }

    private async Task<IEnumerable<OutGenre>> SearchGenre(string? value, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (string.IsNullOrWhiteSpace(value))
        {
            return _availableGenres;
        }

        return _availableGenres.Where(x =>
            x.Name
                .ToLower()
                .Contains(value, StringComparison.InvariantCultureIgnoreCase)
            && !_primaryGenres.Contains(x)
            && !_influenceGenres.Contains(x));
    }

    private void Clean()
    {
        _album = new InAlbum
        {
            PrimaryGenreIds = [],
            InfluenceGenreIds = []
        };

        _primaryGenres.Clear();
        _influenceGenres.Clear();

        _selectedPrimaryGenre = null;
        _selectedPrimaryText = "";
        
        _selectedInfluenceGenre = null;
        _selectedInfluenceText = "";

        _recordingDates = null;
    }
}