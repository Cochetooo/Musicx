using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Albums;

public partial class AlbumEditModal
{
    private ILogger _logger = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private OutArtist? _artist;
    private InAlbum _album = null!;

    private DateRange? _recordingDates;

    private bool _isEditing;
    private bool _isArtworkLoading;
    private bool _showAdvancedFlags;

    private MudTextField<string> _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private MudDialog _modalRef = null!;
    
    private readonly List<OutArtworkCandidate> _artworkCandidates = [];
    private string? _selectedArtworkUrl;
    private IBrowserFile? _pendingArtworkFile;
    private string? _pendingArtworkFileName;

    private string? CurrentArtworkPreview => _pendingArtworkFile is not null ? _selectedArtworkUrl ?? _album.ArtworkUrl : _selectedArtworkUrl ?? _album.ArtworkUrl;


    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(nameof(AlbumEditModal));
        _album = CreateEmptyAlbum();
    }
    
    public async Task Show(OutArtist artist, OutAlbum? album = null)
    {
        _artist = artist;
        _isEditing = album is not null;
        _showAdvancedFlags = false;
        _pendingArtworkFile = null;
        _pendingArtworkFileName = null;
        _artworkCandidates.Clear();

        _album = album?.ToRaw() ?? CreateEmptyAlbum();
        _album.ArtistId = _artist.Id;
        _selectedArtworkUrl = _album.ArtworkUrl;
        _recordingDates = new DateRange(_album.BeginRecordDate, _album.EndRecordDate);
        
        await _modalRef.ShowAsync();
        await InvokeAsync(StateHasChanged);
        await _nameTextEdit.SetTextAsync(_album.Name);

        if (!string.IsNullOrWhiteSpace(_album.Name))
        {
            await FetchArtworkCandidatesAsync();
        }
    }

    private async Task Save()
    {
        _album.BeginRecordDate = _recordingDates?.Start;
        _album.EndRecordDate = _recordingDates?.End;
        
        _album.ArtworkUrl = _selectedArtworkUrl ?? _album.ArtworkUrl;
        
        var response = await UcSave.ExecuteAsync(_album);
        
        if (!response.IsSuccessStatusCode)
        {
            Snackbar.Add($"Could not save album: {response.ReasonPhrase}", Severity.Error);
            return;
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!long.TryParse(responseContent, out var albumId))
        {
            albumId = _album.Id;
        }

        _album.Id = albumId;

        var persistedArtworkUrl = await PersistArtworkAsync(albumId);
        if (!string.IsNullOrWhiteSpace(persistedArtworkUrl) && persistedArtworkUrl != _album.ArtworkUrl)
        {
            _album.ArtworkUrl = persistedArtworkUrl;
            var artworkSaveResponse = await UcSave.ExecuteAsync(_album);
            if (!artworkSaveResponse.IsSuccessStatusCode)
            {
                Snackbar.Add("Album saved but artwork could not be persisted locally.", Severity.Warning);
            }
        }
        Snackbar.Add("Album saved successfully!", Severity.Success);
        
        Clean();
        
        await OnSave.InvokeAsync();
        await Hide();
    }
    
    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }

    private async Task FetchArtworkCandidatesAsync()
    {
        _isArtworkLoading = true;
        await InvokeAsync(StateHasChanged);
        
        _artworkCts?.Cancel();
        _artworkCts?.Dispose();

        _artworkCts = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(_album.Name) || string.IsNullOrWhiteSpace(_artist?.Name))
        {
            _isArtworkLoading = false;
            return;
        }

        try
        {
            await Task.Delay(500, _artworkCts.Token);

            var response = await Http.GetStringAsync(
                $"api/artworks/album-options?name={Uri.EscapeDataString(_album.Name)}&artist={Uri.EscapeDataString(_artist.Name)}",
                _artworkCts.Token);

            var result = JsonConvert.DeserializeObject<FetchAlbumInfoResponse>(response)?.SearchResult;

            _artworkCandidates.Clear();

            if (result?.Candidates is not null)
            {
                _artworkCandidates.AddRange(result.Candidates);
            }

            if (_pendingArtworkFile is null && (string.IsNullOrWhiteSpace(_selectedArtworkUrl) ||
                                                !_selectedArtworkUrl.Contains("/Artists/",
                                                    StringComparison.OrdinalIgnoreCase)))
            {
                _selectedArtworkUrl = _artworkCandidates.FirstOrDefault()?.Url ?? _selectedArtworkUrl;
            }
            
            _isArtworkLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        catch (TaskCanceledException)
        {
            
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Error while retrieving artwork : " + ex.Message);
            _isArtworkLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
    
    private async Task NameTextChanged(string newValue)
    {
        _album.Name = EnglishTitleCaseHelper.ToTitleCase(newValue);

        await FetchArtworkCandidatesAsync();
    }
    
    private void ToggleAdvancedFlags() => _showAdvancedFlags = !_showAdvancedFlags;
    
    private void SelectArtworkCandidate(OutArtworkCandidate candidate)
    {
        _pendingArtworkFile = null;
        _pendingArtworkFileName = null;
        _selectedArtworkUrl = candidate.Url;
    }

    private void OnArtworkFileSelected(InputFileChangeEventArgs args)
    {
        var file = args.File;

        _pendingArtworkFile = file;
        _pendingArtworkFileName = file.Name;
        _selectedArtworkUrl = _artworkCandidates.FirstOrDefault()?.Url ?? _selectedArtworkUrl;
    }
    
    private async Task<string?> PersistArtworkAsync(long albumId)
    {
        if (_artist is null || albumId == 0)
        {
            return _album.ArtworkUrl;
        }

        if (_pendingArtworkFile is not null)
        {
            using var form = new MultipartFormDataContent();
            await using var stream = _pendingArtworkFile.OpenReadStream(10_000_000);
            form.Add(new StreamContent(stream), "file", _pendingArtworkFile.Name);

            var response = await Http.PostAsync($"api/artworks/artists/{_artist.Id}/albums/{albumId}", form);
            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add("Album saved but custom artwork upload failed.", Severity.Warning);
                return _album.ArtworkUrl;
            }

            _pendingArtworkFile = null;
            _pendingArtworkFileName = null;
            _selectedArtworkUrl = await response.Content.ReadAsStringAsync();
            _selectedArtworkUrl = _selectedArtworkUrl.Trim('"');
            return _selectedArtworkUrl;
        }

        if (string.IsNullOrWhiteSpace(_selectedArtworkUrl) || _selectedArtworkUrl.Contains("/Artists/", StringComparison.OrdinalIgnoreCase))
        {
            return _selectedArtworkUrl;
        }

        using var importForm = new MultipartFormDataContent
        {
            { new StringContent(_selectedArtworkUrl), "remoteUrl" }
        };

        var importResponse = await Http.PostAsync($"api/artworks/artists/{_artist.Id}/albums/{albumId}", importForm);
        if (!importResponse.IsSuccessStatusCode)
        {
            Snackbar.Add("Album saved but external artwork import failed.", Severity.Warning);
            return _album.ArtworkUrl;
        }

        _selectedArtworkUrl = (await importResponse.Content.ReadAsStringAsync()).Trim('"');
        return _selectedArtworkUrl;
    }

    private void Clean()
    {
        _album = CreateEmptyAlbum();
        _recordingDates = null;
        _selectedArtworkUrl = null;
        _pendingArtworkFile = null;
        _pendingArtworkFileName = null;
        _artworkCandidates.Clear();
    }
    
    private static InAlbum CreateEmptyAlbum() => new()
    {
        IsVisible = true
    };
}