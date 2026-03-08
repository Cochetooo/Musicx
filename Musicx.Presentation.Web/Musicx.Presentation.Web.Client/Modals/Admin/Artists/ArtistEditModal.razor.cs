using System.Text.Json;
using ISO3166;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Artists;

public partial class ArtistEditModal
{
    private ILogger _logger = null!;
    
    private InArtist _artist = null!;
    private List<OutArtist> _existingArtists = [];

    private bool _isLoading;
    private bool _showOrigin;

    private MudTextField<string> _nameTextEdit = null!;

    private CancellationTokenSource? _artworkCts;

    private List<string> _countries = [];

    private MudDialog _modalRef = null!;
    private bool _isEditing = false;
    
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
        var response = await UcSave.ExecuteAsync(_artist);

        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"Could not save artist: {msg}", Severity.Error);
        }
        else
        {
            Snackbar.Add($"New artist {_artist.Name} created successfully!", Severity.Success);
        }
        
        await OnSave.InvokeAsync();
        await Hide();
    }

    public async Task Show(OutArtist? artist = null)
    {
        await _modalRef.ShowAsync();

        _isEditing = artist is not null;
        if (_isEditing)
        {
            _artist = artist!.ToRaw();
            await _nameTextEdit.SetTextAsync(_artist.Name);
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
            await Task.Delay(1_000, _artworkCts.Token);

            var content = await Http.GetStringAsync(
                $"api/external/artist?name={Uri.EscapeDataString(_artist.Name)}",
                _artworkCts.Token
            );

            var response = JsonSerializer.Deserialize<OutArtist?>(content, JsonHelper.OptionsDefault);

            if (null == response)
            {
                _logger.LogWarning($"⚠️ Artist not found: {_artist.Name}");
            }
            else
            {
                _logger.LogInformation("ℹ️ Retrieved Artwork Url : " + response.ArtworkUrl);
                _artist.ArtworkUrl = response.ArtworkUrl;
            }
            
            _isLoading = false;
            await InvokeAsync(StateHasChanged);

            if (!_isEditing)
            {
                _existingArtists = await UcListExistingArtists.ExecuteAsync(
                    pagingOptions: new PagingOptions(Take: 1, Skip: 0),
                    filter: _artist.Name,
                    filterExact: true);
            }
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
    
    /* private async Task<IEnumerable<string>> SearchRegion(string value)
    {
        if (string.IsNullOrWhiteSpace(_artist.CurrentCountry)) return [];

        var url = $"https://secure.geonames.org/searchJSON?country={_artist.CurrentCountry}&featureCode=ADM1&maxRows=10&username=TON_USER&q={value}";
        var json = await _http.GetFromJsonAsync<GeoNamesResponse>(url);
        return json?.geonames.Select(g => g.name) ?? [];
    } */
    
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