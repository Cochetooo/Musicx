using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Client.Modals.Admin.Artists;
using Musicx.Presentation.Web.Client.Models;

namespace Musicx.Presentation.Web.Client.Pages.Admin;

public partial class ArtistDashboard
{
    private ViewMode _viewMode = ViewMode.Normal;
    
    private ArtistEditModal _artistEditModal = null!;
    private MudDialog _confirmDeleteModal = null!;

    private ILogger _logger = null!;

    private List<OutArtist> _artists = [];
    private List<OutArtist> _filteredArtists = [];
    private HashSet<OutArtist> _selectedArtists = [];

    private string _searchDataGrid = null!;

    private List<BreadcrumbItem> _breadcrumb =
    [
        new("Musicx", href: "/"),
        new("Admin", href: "#"),
        new("Artist Management", href: "#")
    ];
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        
        _logger = LoggerProvider.CreateLogger(nameof(ArtistDashboard));
        
        await LoadData();
    }

    private async Task LoadData()
    {
        _logger.LogInformation("🔄️ ArtistDashboard : UPDATE Data");

        _artists = await UcList.ExecuteAsync(take: 1000);
        _filteredArtists = new List<OutArtist>(_artists);
        
        await InvokeAsync(StateHasChanged);
    }

    private void ShowAddModal()
    {
        _artistEditModal?.Show();
    }

    private void ShowEditModal(OutArtist artist)
    {
        _selectedArtists.Clear();
        _selectedArtists.Add(artist);

        _artistEditModal?.Show(artist);
    }

    private async Task DeleteData()
    {
        foreach (var artist in _selectedArtists)
        {
            await UcDelete.ExecuteAsync(artist.Id);
        }
        
        await HideDeleteModal();
        await LoadData();
        EnableNormalMode();
    }

    private void EnableDeleteMode()
    {
        _selectedArtists.Clear();
        _viewMode = ViewMode.Delete;
    }
    
    private void EnableNormalMode()
    {
        _selectedArtists.Clear();
        _viewMode = ViewMode.Normal;
    }

    private async Task ShowDeleteModal()
    {
        await _confirmDeleteModal.ShowAsync();
    }

    private async Task HideDeleteModal()
    {
        await _confirmDeleteModal.CloseAsync();
    }

    private async Task OnQuitModal()
    {
        EnableNormalMode();
        await LoadData();
    }

    private void ApplySearchFilters()
    {
        _logger.LogDebug($"🔎 ArtistDashboard : Apply Search Filters: {_searchDataGrid}");

        var keywords = string.IsNullOrWhiteSpace(_searchDataGrid)
            ? []
            : _searchDataGrid.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        _filteredArtists = _artists
            .Where(a =>
                keywords.Length == 0
                || keywords.All(keyword =>
                    a.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    private void KeyDownSearchInput(KeyboardEventArgs keyEvent)
    {
        if (keyEvent.Key == "Enter")
        {
            ApplySearchFilters();
        }
    }
}