using System.Text.Json;
using Blazorise;
using Microsoft.AspNetCore.Components;
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

    private bool _isLoading;
    private bool _isConfirmable;

    private TextEdit _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private Modal _modalRef = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        
        _logger = LoggerProvider.CreateLogger(nameof(AlbumEditModal));

        _album = new InAlbum();
        _availableGenres = await UcListGenres.ExecuteAsync(take: 10_000);
    }

    private async Task Save()
    {
        _album.PrimaryGenreIds = _primaryGenres.Select(g => g.Id).ToList();
        _album.InfluenceGenreIds = _influenceGenres.Select(g => g.Id).ToList();
        
        _logger.LogInformation($"⛏️ AlbumEditModal : Persisting primary genres {string.Join(",", _album.PrimaryGenreIds)} " +
                               $"and influences {string.Join(",", _album.InfluenceGenreIds)}");
        
        await UcSave.ExecuteAsync(_album);
        await OnSave.InvokeAsync();

        _album.PrimaryGenreIds = [];
        _album.InfluenceGenreIds = [];
        
        _primaryGenres.Clear();
        _influenceGenres.Clear();
        
        Hide();
    }

    public void Show(OutArtist artist, OutAlbum? album = null)
    {
        if (null != album)
        {
            _album = album.ToRaw();
            _nameTextEdit.Text = album.Name;
            _nameTextEdit.Revalidate();
            _primaryGenres = album.PrimaryGenres?.ToList() ?? [];
            _influenceGenres = album.InfluenceGenres?.ToList() ?? [];
            StateHasChanged();
        }
        
        _artist = artist;
        _album.ArtistId = _artist.Id;
        _modalRef.Show();
    }
    
    private void Hide()
    {
        _modalRef.Hide();
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
    
    private void ValidateNonEmptyField(ValidatorEventArgs e)
    {
        _isConfirmable = string.IsNullOrWhiteSpace(Convert.ToString(e.Value));
        
        e.Status = _isConfirmable
            ? ValidationStatus.None
            : ValidationStatus.Success;
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
}