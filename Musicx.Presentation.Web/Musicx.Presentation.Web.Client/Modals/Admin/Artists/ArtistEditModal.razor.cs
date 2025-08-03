using System.Text.Json;
using Blazorise;
using ISO3166;
using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Artists;

public partial class ArtistEditModal
{
    private ILogger _logger = null!;
    
    private InArtist _artist = null!;

    private bool _isLoading;
    private bool _isConfirmable;

    private TextEdit _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private List<string> _countries = [];

    private Modal _modalRef = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(ArtistEditModal));

        _artist = new InArtist
        {
            Discriminator = ArtistDiscriminator.Artist
        };

        _countries = Country.List
            .Select(c => c.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToList();
    }

    private async Task Save()
    {
        await UcSave.ExecuteAsync(_artist);
        await OnSave.InvokeAsync();
        Hide();
    }

    public void Show(OutArtist? artist = null)
    {
        if (null != artist)
        {
            _artist = artist.ToRaw();
            _nameTextEdit.Text = _artist.Name;
            _nameTextEdit.Revalidate();
            StateHasChanged();
        }
        
        _modalRef.Show();
    }

    private void Hide()
    {
        _modalRef.Hide();
    }

    private void ValidateNonEmptyField(ValidatorEventArgs e)
    {
        _isConfirmable = string.IsNullOrWhiteSpace(Convert.ToString(e.Value));
        
        e.Status = _isConfirmable
            ? ValidationStatus.None
            : ValidationStatus.Success;
    }

    private async Task UpdateArtwork()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);
        _artworkCts?.Cancel();
        _artworkCts?.Dispose();

        _artworkCts = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(_artist.Name))
        {
            _isLoading = false;
            return;
        }

        try
        {
            await Task.Delay(2000, _artworkCts.Token);

            var content = await Http.GetStringAsync(
                $"api/external/artist?name={Uri.EscapeDataString(_artist.Name)}",
                _artworkCts.Token
            );

            var response = JsonSerializer.Deserialize<OutArtist?>(content, JsonHelper.OptionsDefault);

            if (null == response)
            {
                _logger.LogWarning($"⚠️ Artist not found: {_artist.Name}");
                _isLoading = false;
                return;
            }

            _logger.LogInformation("ℹ️ Retrieved Artwork Url : " + response.ArtworkUrl);
            _artist.ArtworkUrl = response.ArtworkUrl;
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
        _artist.Name = newValue;

        await UpdateArtwork();
    }
}