using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;
using Musicx.Presentation.Web.Client.Modals.Admin.Genres;
using Musicx.Presentation.Web.Client.Models;
using GenreJoinSpecification = Musicx.Application.Desktop.Specifications.GenreJoinSpecification;

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

    private string _searchDataGrid = string.Empty;

    private List<BreadcrumbItem> _breadcrumb =
    [
        new("Musicx", href: "/"),
        new("Admin", href: "#"),
        new("Genre Management", href: "#")
    ];

    protected override void OnInitialized()
    {
        _logger = LoggerProvider.CreateLogger(nameof(GenreDashboard));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        
        await LoadData();
    }

    private async Task LoadData()
    {
        _logger.LogInformation("🔄️ GenreDashboard : UPDATE Data");

        _genres = await UcList.ExecuteAsync(
            order: new GenreOrderSpecification
            {
                CanonicalName = 1
            },
            pagingOptions: new PagingOptions(Take: 10_000, Skip: 0)
        );
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

    private async Task<GridData<OutGenre>> LoadGenresData(GridState<OutGenre> state)
    {
        var response = await UcList.ExecuteAsync(
            filterExact: true,
            filter: _searchDataGrid,
            pagingOptions: new PagingOptions(Take: state.PageSize, Skip: state.Page * state.PageSize),
            order: new GenreOrderSpecification
            {
                CanonicalName = 1,
                ShortName = 2
            },
            joins: new GenreJoinSpecification
            {
                IncludeParents = true
            }
        );

        return new GridData<OutGenre>
        {
            TotalItems = response.Count,
            Items = response
        };
    }

    private void ApplySearchFilters()
    {
        _logger.LogDebug("🔎 GenreDashboard : Apply Search Filters: {Search}", _searchDataGrid);

        _filteredGenres = SearchKeywordHelper.FilterByKeywords(
            _genres,
            _searchDataGrid,
            x => x.CanonicalName
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