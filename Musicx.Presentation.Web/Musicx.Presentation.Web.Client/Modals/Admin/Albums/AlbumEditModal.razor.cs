using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
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

    private DateRange? _recordingDates;

    private bool _isLoading;

    private MudTextField<string> _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private MudDialog _modalRef = null!;

    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(nameof(AlbumEditModal));
        _album = new InAlbum();
    }

    private async Task Save()
    {
        _album.BeginRecordDate = _recordingDates?.Start;
        _album.EndRecordDate = _recordingDates?.End;
        
        _logger.LogInformation($"💾 Saving album {_album.Name}...");

        var response = await UcSave.ExecuteAsync(_album);
        
        if (!response.IsSuccessStatusCode)
        {
            Snackbar.Add($"Could not save album: {response.ReasonPhrase}", Severity.Error);
            return;
        }

        _logger.LogInformation("✅ Album saved successfully!");
        Snackbar.Add("Album saved successfully!", Severity.Success);
        
        Clean();
        
        await OnSave.InvokeAsync();
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

            _recordingDates = new DateRange(
                _album.BeginRecordDate,
                _album.EndRecordDate
            );
            
            await InvokeAsync(StateHasChanged);
            
            await _nameTextEdit.SetTextAsync(album.Name);
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
            await Task.Delay(1_000, _artworkCts.Token);

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
        _album.Name = EnglishTitleCaseHelper.ToTitleCase(newValue);

        await UpdateArtwork();
    }

    private void Clean()
    {
        _album = new InAlbum();

        _recordingDates = null;
    }
}