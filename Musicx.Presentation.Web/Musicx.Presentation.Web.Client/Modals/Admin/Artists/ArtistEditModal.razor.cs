using System.Text.Json;
using ISO3166;
using Microsoft.AspNetCore.Components;
using MudBlazor;
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

    private MudTextField<string> _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private List<string> _countries = [];

    private MudDialog _modalRef = null!;
    
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
        await Hide();
    }

    public async Task Show(OutArtist? artist = null)
    {
        await _modalRef.ShowAsync();
        
        if (null != artist)
        {
            _artist = artist.ToRaw();
            await _nameTextEdit.SetText(_artist.Name);
            // _nameTextEdit.Revalidate();
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            _artist = new InArtist
            {
                Discriminator = ArtistDiscriminator.Artist
            };
        }
    }

    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }

    /* private void ValidateNonEmptyField(ValidatorEventArgs e)
    {
        _isConfirmable = string.IsNullOrWhiteSpace(Convert.ToString(e.Value));
        
        e.Status = _isConfirmable
            ? ValidationStatus.None
            : ValidationStatus.Success;
    } */

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
    
    private async Task<IEnumerable<string>>? SearchCountry(string? value, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (string.IsNullOrWhiteSpace(value))
        {
            return _countries;
        }

        return _countries.Where(x =>
            x
                .ToLower()
                .Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }
}