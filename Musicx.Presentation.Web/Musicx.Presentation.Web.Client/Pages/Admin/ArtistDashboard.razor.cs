using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Infrastructure.API.Persistence.Specifications.Artist;
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

    private string _searchDataGrid = string.Empty;

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

        _artists = (await Api.FindAsync<InArtist, OutArtist>(
            order: new ArtistOrderSpecification
            {
                Name = 1
            },
            pagingOptions: new PagingOptions(Take: 1_000, Skip: 0)
        )).Items;
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
            await Api.DeleteAsync<InArtist>(artist.Id);
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
        _logger.LogDebug("🔎 ArtistDashboard : Apply Search Filters: {Search}", _searchDataGrid);

        _filteredArtists = SearchKeywordHelper.FilterByKeywords(
            _artists,
            _searchDataGrid,
            x => x.Name
        );
    }

    private void KeyDownSearchInput(KeyboardEventArgs keyEvent)
    {
        if (keyEvent.Key == "Enter")
        {
            ApplySearchFilters();
        }
    }
}