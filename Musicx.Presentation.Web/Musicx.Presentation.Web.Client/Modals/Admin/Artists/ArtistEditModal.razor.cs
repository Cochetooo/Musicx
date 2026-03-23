using System.Net.Http.Json;
using System.Text.Json;
using ISO3166;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Newtonsoft.Json;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Artists;

public partial class ArtistEditModal
{
    private readonly (ArtistDiscriminator Value, string Title, string Description, string Icon)[] _artistTypeOptions =
    [
        (ArtistDiscriminator.Artist, "Generic", "Collaborative project or undefined biography.", Icons.Material.Filled.AutoAwesome),
        (ArtistDiscriminator.Band, "Band", "Band, formation or collective with a start and/or a split date", Icons.Material.Filled.Groups),
        (ArtistDiscriminator.Person, "Artist", "Solo Artist with a civil identity & age", Icons.Material.Filled.Person)
    ];
    
    private ILogger _logger = null!;
    
    private InArtist _artist = null!;
    private readonly List<OutArtist> _existingArtists = [];
    private readonly List<OutArtworkCandidate> _artworkCandidates = [];
    private readonly List<string> _countries = [];

    private bool _isArtworkLoading;
    private bool _showOrigin;
    private bool _isEditing;
    private int _wizardStep;

    private MudTextField<string> _nameTextEdit = null!;
    private bool _hydrateIdentityName;
    private CancellationTokenSource? _artworkCts;
    private MudDialog _modalRef = null!;
    
    private string? _selectedArtworkUrl;
    private IBrowserFile? _pendingArtworkFile;
    private string? _pendingArtworkFileName;
    
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    
    private bool CanContinueIdentityStep => !string.IsNullOrWhiteSpace(_artist.Name);
    private string? CurrentArtworkPreview => _selectedArtworkUrl ?? _artist.ArtworkUrl;

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(ArtistEditModal));

        _artist = CreateEmptyArtist();

        _countries.AddRange(Country.List
            .Select(c => c.Name)
            .Distinct()
            .OrderBy(name => name));
    }
    
    public async Task Show(OutArtist? artist = null)
    {
        _isEditing = artist is not null;
        _wizardStep = _isEditing ? 2 : 1;
        _pendingArtworkFile = null;
        _pendingArtworkFileName = null;
        _existingArtists.Clear();
        _artworkCandidates.Clear();

        _artist = artist?.ToRaw() ?? CreateEmptyArtist();
        _selectedArtworkUrl = _artist.ArtworkUrl;
        _showOrigin = HasOriginData(_artist);
        
        await _modalRef.ShowAsync();
        await InvokeAsync(StateHasChanged);

        _hydrateIdentityName = _wizardStep == 1;

        if (!string.IsNullOrWhiteSpace(_artist.Name))
        {
            await FetchArtworkCandidatesAsync();
            await RefreshExistingArtistsAsync();
        }
    }

    private async Task Save()
    {
        _artist.ArtworkUrl = _selectedArtworkUrl ?? _artist.ArtworkUrl;
        var response = await UcSave.ExecuteAsync(_artist);

        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"Could not save artist: {msg}", Severity.Error);
            return;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!long.TryParse(responseContent, out var artistId))
        {
            artistId = _artist.Id;
        }
        
        _artist.Id = artistId;
        
        var persistedArtworkUrl = await PersistArtworkAsync(artistId);
        if (!string.IsNullOrWhiteSpace(persistedArtworkUrl) && persistedArtworkUrl != _artist.ArtworkUrl)
        {
            _artist.ArtworkUrl = persistedArtworkUrl;
            var artworkSaveResponse = await UcSave.ExecuteAsync(_artist);
            if (!artworkSaveResponse.IsSuccessStatusCode)
            {
                Snackbar.Add("Artist saved but artwork could not be persisted locally.", Severity.Warning);
            }
        }

        Snackbar.Add(_isEditing
            ? $"Artist {_artist.Name} updated successfully!"
            : $"New artist {_artist.Name} created successfully!", Severity.Success);

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

        if (string.IsNullOrWhiteSpace(_artist.Name))
        {
            _isArtworkLoading = false;
            return;
        }

        try
        {
            await Task.Delay(500, _artworkCts.Token);
            
            var response = await Http.GetStringAsync(
                $"api/artworks/artist-options?name={Uri.EscapeDataString(_artist.Name)}",
                _artworkCts.Token);

            var result = JsonConvert.DeserializeObject<FetchArtistInfoResponse>(response)?.SearchResult;

            _artworkCandidates.Clear();
            
            if (result?.Candidates is not null)
            {
                _logger.LogInformation("adding range candidates.");
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
        _artist.Name = EnglishTitleCaseHelper.ToTitleCase(newValue);

        await FetchArtworkCandidatesAsync();
        await RefreshExistingArtistsAsync();
    }

    private async Task RefreshExistingArtistsAsync()
    {
        _existingArtists.Clear();

        if (_isEditing || string.IsNullOrWhiteSpace(_artist.Name))
        {
            return;
        }

        var artists = await UcListExistingArtists.ExecuteAsync(
            pagingOptions: new PagingOptions(Take: 3, Skip: 0),
            filter: _artist.Name,
            filterExact: true);

        _existingArtists.AddRange(artists);
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

        return _countries.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }
    
    private void SelectArtistType(ArtistDiscriminator discriminator) => _artist.Discriminator = discriminator;
    private void ToggleOrigin() => _showOrigin = !_showOrigin;
    private void GoToDetailsStep()
    {
        _wizardStep = 2;
    }

    private void GoBackToIdentityStep()
    {
        _wizardStep = 1;
        _hydrateIdentityName = true;
    }

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
        _selectedArtworkUrl = _selectedArtworkUrl ?? _artworkCandidates.FirstOrDefault()?.Url;
    }

    private async Task<string?> PersistArtworkAsync(long artistId)
    {
        if (artistId == 0)
        {
            return _artist.ArtworkUrl;
        }

        if (_pendingArtworkFile is not null)
        {
            using var form = new MultipartFormDataContent();
            await using var stream = _pendingArtworkFile.OpenReadStream(10_000_000);
            form.Add(new StreamContent(stream), "file", _pendingArtworkFile.Name);

            var response = await Http.PostAsync($"api/artworks/artists/{artistId}", form);
            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add("Artist saved but custom artwork upload failed.", Severity.Warning);
                return _artist.ArtworkUrl;
            }

            _pendingArtworkFile = null;
            _pendingArtworkFileName = null;
            _selectedArtworkUrl = (await response.Content.ReadAsStringAsync()).Trim('"');
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

        var importResponse = await Http.PostAsync($"api/artworks/artists/{artistId}", importForm);
        if (!importResponse.IsSuccessStatusCode)
        {
            Snackbar.Add("Artist saved but external artwork import failed.", Severity.Warning);
            return _artist.ArtworkUrl;
        }

        _selectedArtworkUrl = (await importResponse.Content.ReadAsStringAsync()).Trim('"');
        return _selectedArtworkUrl;
    }

    private static InArtist CreateEmptyArtist() => new()
    {
        Discriminator = ArtistDiscriminator.Artist,
        IsVisible = true
    };

    private static bool HasOriginData(InArtist artist)
        => !string.IsNullOrWhiteSpace(artist.OriginCountry)
           || !string.IsNullOrWhiteSpace(artist.OriginRegion)
           || !string.IsNullOrWhiteSpace(artist.OriginTown);
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_hydrateIdentityName)
        {
            _hydrateIdentityName = false;
            await _nameTextEdit.SetTextAsync(_artist.Name);
        }
    }
}