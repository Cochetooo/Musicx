using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Client.Modals.Admin.Genres;
using Musicx.Presentation.Web.Client.Models;

namespace Musicx.Presentation.Web.Client.Pages.Admin;

public partial class GenreDashboard
{
    private ViewMode _viewMode = ViewMode.Normal;
    
    private GenreEditModal _genreEditModal = null!;
    private MudDialog _confirmDeleteModal = null!;

    private ILogger _logger = null!;

    private List<OutGenre> _genres = [];
    private List<OutGenre> _filteredGenres = [];
    private HashSet<OutGenre> _selectedGenres = [];

    private string _searchDataGrid = null!;

    private List<BreadcrumbItem> _breadcrumb =
    [
        new("Musicx", href: "/"),
        new("Admin", href: "#"),
        new("Genre Management", href: "#")
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
        _logger.LogInformation("🔄️ GenreDashboard : UPDATE Data");

        _genres = await UcList.ExecuteAsync(take: 10_000);
        _filteredGenres = new List<OutGenre>(_genres);
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task ShowAddModal()
    {
        await _genreEditModal.Show();
    }

    private async Task ShowEditModal(OutGenre genre)
    {
        _selectedGenres.Clear();
        _selectedGenres.Add(genre);

        await _genreEditModal.Show(genre);
    }

    private async Task DeleteData()
    {
        foreach (var genre in _selectedGenres)
        {
            await UcDelete.ExecuteAsync(genre.Id);
        }
        
        await HideDeleteModal();
        await LoadData();
        EnableNormalMode();
    }

    private void EnableDeleteMode()
    {
        _selectedGenres.Clear();
        _viewMode = ViewMode.Delete;
    }
    
    private void EnableNormalMode()
    {
        _selectedGenres.Clear();
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
        _logger.LogDebug($"🔎 GenreDashboard : Apply Search Filters: {_searchDataGrid}");

        var keywords = string.IsNullOrWhiteSpace(_searchDataGrid)
            ? []
            : _searchDataGrid.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        _filteredGenres = _genres
            .Where(a =>
                keywords.Length == 0
                || keywords.All(keyword =>
                    a.CanonicalName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
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