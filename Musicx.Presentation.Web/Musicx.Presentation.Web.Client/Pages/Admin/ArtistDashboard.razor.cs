using Blazorise;
using Microsoft.AspNetCore.Components.Web;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Client.Modals.Admin.Artists;
using Musicx.Presentation.Web.Client.Models;

namespace Musicx.Presentation.Web.Client.Pages.Admin;

public partial class ArtistDashboard
{
    private ViewMode _viewMode = ViewMode.Normal;
    
    private ArtistEditModal? _artistEditModal;
    private Modal? _confirmDeleteModal;

    private ILogger _logger = null!;

    private List<OutArtist> _artists = [];
    private List<OutArtist> _filteredArtists = [];
    private List<OutArtist> _selectedArtists = [];

    private string _searchDataGrid = null!;
    
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

        _artists = await UcList.ExecuteAsync(take: 100);
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

    private Task ShowDeleteModal()
    {
        return _confirmDeleteModal!.Show();
    }

    private Task HideDeleteModal()
    {
        return _confirmDeleteModal!.Hide();
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