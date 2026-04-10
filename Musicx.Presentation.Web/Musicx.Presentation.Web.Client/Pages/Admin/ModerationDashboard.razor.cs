using MudBlazor;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Artist;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;
using Musicx.Infrastructure.API.Persistence.Specifications.Security;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Presentation.Web.Client.Modals.Admin.Artists;
using Musicx.Presentation.Web.Client.Modals.Admin.Genres;

namespace Musicx.Presentation.Web.Client.Pages.Admin;

public partial class ModerationDashboard
{
    private enum ModerationSection { Overview, Artists, Albums, Genres, Users, RolesPermissions }

    private ILogger _logger = null!;

    private ModerationSection _section = ModerationSection.Overview;

    private ArtistEditModal _artistEditModal = null!;
    private GenreEditModal _genreEditModal = null!;
    
    private List<OutRole> _roles = [];
    private List<OutPermission> _permissions = [];
    
    private bool _overviewLoaded;
    private bool _artistsLoaded;
    private bool _genresLoaded;
    private bool _albumsLoaded;
    private bool _usersLoaded;
    private bool _rolesLoaded;

    private List<OutArtist> _pendingArtists = [];
    private List<OutGenre> _pendingGenres = [];
    private List<OutUser> _pagedUsers = [];
    private int _usersTotal;

    private string _artistSearch = string.Empty;
    private string _genreSearch = string.Empty;
    private string _albumSearch = string.Empty;
    private string _userSearch = string.Empty;

    private int _artistsToday;
    private int _artistsWeek;
    private int _artistsMonth;
    private int _artistsYear;
    private int _artistsVisible;
    private int _artistsHidden;
    private int _artistsTotal;

    private int _genresToday;
    private int _genresWeek;
    private int _genresMonth;
    private int _genresYear;
    private int _genresVisible;
    private int _genresHidden;
    private int _genresTotal;

    private int _albumsToday;
    private int _albumsVisible;
    private int _albumsHidden;
    private int _albumsTotal;

    private int _usersToday;
    private int _usersCount;
    
    private int _totalPendingItems;
    private int _artistHiddenRatio;
    private int _genreHiddenRatio;

    private bool IsAdmin => UserClientContext.Roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                            || UserClientContext.Can("admin.role.read");

    private IReadOnlyList<OutArtist> PendingArtists => _pendingArtists;
    private IReadOnlyList<OutGenre> PendingGenres => _pendingGenres;

    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(typeof(ModerationDashboard));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await EnsureSectionLoaded(_section);
        await InvokeAsync(StateHasChanged);
    }

    private void SetSection(ModerationSection section)
    {
        _section = section;
        _ = InvokeAsync(async () =>
        {
            await EnsureSectionLoaded(section);
            StateHasChanged();
        });
    }

    private async Task EnsureSectionLoaded(ModerationSection section)
    {
        switch (section)
        {
            case ModerationSection.Overview:
                await LoadOverviewData(force: false);
                break;
            case ModerationSection.Artists:
                await LoadArtistsSectionData(force: false);
                break;
            case ModerationSection.Albums:
                await LoadAlbumsSectionData(force: false);
                break;
            case ModerationSection.Genres:
                await LoadGenresSectionData(force: false);
                break;
            case ModerationSection.Users:
                await LoadUsersSectionData(force: false);
                if (IsAdmin)
                {
                    await LoadRolesAndPermissions(force: false);
                }

                break;
            case ModerationSection.RolesPermissions:
                await LoadRolesAndPermissions(force: false);
                break;
        }
    }

    private async Task HandlePanelEditSaved()
    {
        await RefreshCurrentSection(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task RefreshCurrentSection(bool force = true)
    {
        switch (_section)
        {
            case ModerationSection.Overview:
                await LoadOverviewData(force);
                break;
            case ModerationSection.Artists:
                await LoadArtistsSectionData(force);
                break;
            case ModerationSection.Albums:
                await LoadAlbumsSectionData(force);
                break;
            case ModerationSection.Genres:
                await LoadGenresSectionData(force);
                break;
            case ModerationSection.Users:
                await LoadUsersSectionData(force);
                if (IsAdmin)
                {
                    await LoadRolesAndPermissions(force);
                }

                break;
            case ModerationSection.RolesPermissions:
                await LoadRolesAndPermissions(force);
                break;
        }
    }

    private async Task LoadOverviewData(bool force)
    {
        if (!force && _overviewLoaded)
        {
            return;
        }
        
        await LoadAllStats();
        await LoadPendingLists();

        _overviewLoaded = true;
    }
    
    private async Task LoadArtistsSectionData(bool force)
    {
        if (!force && _artistsLoaded)
        {
            return;
        }

        await LoadArtistStats();
        _pendingArtists = (await Api.FindAsync<InArtist, OutArtist>(
            query: new ArtistFindQuery { IsVisible = false },
            pagingOptions: new PagingOptions(25, 0),
            order: new ArtistOrderSpecification { CreatedAt = -1 })).Items;

        _artistsLoaded = true;
    }

    private async Task LoadGenresSectionData(bool force)
    {
        if (!force && _genresLoaded)
        {
            return;
        }

        await LoadGenreStats();
        _pendingGenres = (await Api.FindAsync<InGenre, OutGenre>(
            query: new GenreFindQuery { IsVisible = false },
            pagingOptions: new PagingOptions(25, 0),
            order: new GenreOrderSpecification { CreatedAt = -1 })).Items;
        _genresLoaded = true;
    }

    private async Task LoadAlbumsSectionData(bool force)
    {
        if (!force && _albumsLoaded)
        {
            return;
        }

        await LoadAlbumStats();
        _albumsLoaded = true;
    }
    
    private async Task LoadUsersSectionData(bool force)
    {
        if (!force && _usersLoaded)
        {
            return;
        }

        await LoadUserStats();
        _usersLoaded = true;
    }
    
    private async Task LoadRolesAndPermissions(bool force)
    {
        if (!force && _rolesLoaded)
        {
            return;
        }

        var rolesTask = Api.FindAsync<InRole, OutRole>(pagingOptions: new PagingOptions(10_000, 0), joins: new RoleJoinSpecification { IncludePermissions = true });
        var permissionsTask = Api.FindAsync<InPermission, OutPermission>(pagingOptions: new PagingOptions(10_000, 0));

        await Task.WhenAll(rolesTask, permissionsTask);

        _roles = rolesTask.Result.Items;
        _permissions = permissionsTask.Result.Items;
        _rolesLoaded = true;
    }
    
    private async Task LoadAllStats()
    {
        await Task.WhenAll(
            LoadArtistStats(),
            LoadGenreStats(),
            LoadAlbumStats(),
            LoadUserStats());
        
        ComputeGlobalStats();
    }

    private async Task LoadArtistStats()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var yearStart = new DateTime(now.Year, 1, 1);
        
        var totalTask = Api.CountAsync<InArtist, OutArtist>();
        var visibleTask = Api.CountAsync<InArtist, OutArtist>(new ArtistFindQuery { IsVisible = true });
        var hiddenTask = Api.CountAsync<InArtist, OutArtist>(new ArtistFindQuery { IsVisible = false });
        var todayTask = Api.CountAsync<InArtist, OutArtist>(new ArtistFindQuery { CreatedAtFrom = today });
        var weekTask = Api.CountAsync<InArtist, OutArtist>(new ArtistFindQuery { CreatedAtFrom = weekStart });
        var monthTask = Api.CountAsync<InArtist, OutArtist>(new ArtistFindQuery { CreatedAtFrom = monthStart });
        var yearTask = Api.CountAsync<InArtist, OutArtist>(new ArtistFindQuery { CreatedAtFrom = yearStart });

        await Task.WhenAll(totalTask, visibleTask, hiddenTask, todayTask, weekTask, monthTask, yearTask);

        _artistsTotal = (int)totalTask.Result;
        _artistsVisible = (int)visibleTask.Result;
        _artistsHidden = (int)hiddenTask.Result;
        _artistsToday = (int)todayTask.Result;
        _artistsWeek = (int)weekTask.Result;
        _artistsMonth = (int)monthTask.Result;
        _artistsYear = (int)yearTask.Result;
        _artistHiddenRatio = _artistsTotal > 0 ? (int)Math.Round((double)_artistsHidden * 100 / _artistsTotal) : 0;
    }
    
    private async Task LoadGenreStats()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var yearStart = new DateTime(now.Year, 1, 1);

        var totalTask = Api.CountAsync<InGenre, OutGenre>();
        var visibleTask = Api.CountAsync<InGenre, OutGenre>(new GenreFindQuery { IsVisible = true });
        var hiddenTask = Api.CountAsync<InGenre, OutGenre>(new GenreFindQuery { IsVisible = false });
        var todayTask = Api.CountAsync<InGenre, OutGenre>(new GenreFindQuery { CreatedAtFrom = today });
        var weekTask = Api.CountAsync<InGenre, OutGenre>(new GenreFindQuery { CreatedAtFrom = weekStart });
        var monthTask = Api.CountAsync<InGenre, OutGenre>(new GenreFindQuery { CreatedAtFrom = monthStart });
        var yearTask = Api.CountAsync<InGenre, OutGenre>(new GenreFindQuery { CreatedAtFrom = yearStart });

        await Task.WhenAll(totalTask, visibleTask, hiddenTask, todayTask, weekTask, monthTask, yearTask);

        _genresTotal = (int)totalTask.Result;
        _genresVisible = (int)visibleTask.Result;
        _genresHidden = (int)hiddenTask.Result;
        _genresToday = (int)todayTask.Result;
        _genresWeek = (int)weekTask.Result;
        _genresMonth = (int)monthTask.Result;
        _genresYear = (int)yearTask.Result;
        _genreHiddenRatio = _genresTotal > 0 ? (int)Math.Round((double)_genresHidden * 100 / _genresTotal) : 0;
    }
    
    private async Task LoadAlbumStats()
    {
        var today = DateTime.UtcNow.Date;

        var totalTask = Api.CountAsync<InAlbum, OutAlbum>();
        var visibleTask = Api.CountAsync<InAlbum, OutAlbum>(new AlbumFindQuery { IsVisible = true });
        var hiddenTask = Api.CountAsync<InAlbum, OutAlbum>(new AlbumFindQuery { IsVisible = false });
        var todayTask = Api.CountAsync<InAlbum, OutAlbum>(new AlbumFindQuery { CreatedAtFrom = today });

        await Task.WhenAll(totalTask, visibleTask, hiddenTask, todayTask);

        _albumsTotal = (int)totalTask.Result;
        _albumsVisible = (int)visibleTask.Result;
        _albumsHidden = (int)hiddenTask.Result;
        _albumsToday = (int)todayTask.Result;
    }

    private async Task LoadUserStats()
    {
        var today = DateTime.UtcNow.Date;

        var totalTask = Api.CountAsync<InUser, OutUser>();
        var todayTask = Api.CountAsync<InUser, OutUser>(new UserFindQuery { CreatedAtFrom = today });

        await Task.WhenAll(totalTask, todayTask);

        _usersCount = (int)totalTask.Result;
        _usersToday = (int)todayTask.Result;
    }
    
    private async Task LoadPendingLists()
    {
        var artistsPendingTask = Api.FindAsync<InArtist, OutArtist>(
            query: new ArtistFindQuery { IsVisible = false },
            pagingOptions: new PagingOptions(25, 0),
            order: new ArtistOrderSpecification { CreatedAt = -1 });

        var genresPendingTask = Api.FindAsync<InGenre, OutGenre>(
            query: new GenreFindQuery { IsVisible = false },
            pagingOptions: new PagingOptions(25, 0),
            order: new GenreOrderSpecification { CreatedAt = -1 });

        await Task.WhenAll(artistsPendingTask, genresPendingTask);

        _pendingArtists = artistsPendingTask.Result.Items;
        _pendingGenres = genresPendingTask.Result.Items;
    }

    private void ComputeGlobalStats()
    {
        _totalPendingItems = _artistsHidden + _genresHidden + _albumsHidden;
    }

    private async Task<TableData<OutArtist>> LoadArtists(TableState state, CancellationToken token)
    {
        await EnsureSectionLoaded(ModerationSection.Artists);

        var query = new ArtistFindQuery
        {   
            RawSearch = string.IsNullOrWhiteSpace(_artistSearch) ? null : new TextFilter(_artistSearch)
        };
        
        var response = await Api.FindAsync<InArtist, OutArtist>(
            query: query,
            pagingOptions: new PagingOptions(state.PageSize, state.Page * state.PageSize),
            order: new ArtistOrderSpecification { CreatedAt = -1 });

        token.ThrowIfCancellationRequested();
        
        return new TableData<OutArtist>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }

    private async Task<TableData<OutGenre>> LoadGenres(TableState state, CancellationToken token)
    {
        await EnsureSectionLoaded(ModerationSection.Genres);

        var query = new GenreFindQuery
        {
            RawSearch = string.IsNullOrWhiteSpace(_genreSearch) ? null : new TextFilter(_genreSearch)
        };

        var response = await Api.FindAsync<InGenre, OutGenre>(
            query: query,
            pagingOptions: new PagingOptions(state.PageSize, state.Page * state.PageSize),
            order: new GenreOrderSpecification { CreatedAt = -1 });

        token.ThrowIfCancellationRequested();
        
        return new TableData<OutGenre>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }

    private async Task<TableData<OutAlbum>> LoadAlbums(TableState state, CancellationToken token)
    {
        await EnsureSectionLoaded(ModerationSection.Albums);

        var query = new AlbumFindQuery
        {
            RawSearch = string.IsNullOrWhiteSpace(_albumSearch) ? null : new TextFilter(_albumSearch)
        };
        
        var response = await Api.FindAsync<InAlbum, OutAlbum>(
            query: query,
            pagingOptions: new PagingOptions(state.PageSize, state.Page * state.PageSize),
            order: new AlbumOrderSpecification { CreatedAt = -1 },
            joins: new AlbumJoinSpecification { IncludeArtist = true });

        token.ThrowIfCancellationRequested();
        
        return new TableData<OutAlbum>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }

    private async Task<TableData<OutUser>> LoadUsers(TableState state, CancellationToken token)
    {
        await EnsureSectionLoaded(ModerationSection.Users);

        var query = new UserFindQuery
        {
            RawSearch = string.IsNullOrWhiteSpace(_userSearch) ? null : new TextFilter(_userSearch)
        };
        
        var response = await Api.FindAsync<InUser, OutUser>(
            query: query,
            pagingOptions: new PagingOptions(state.PageSize, state.Page * state.PageSize),
            order: new UserOrderSpecification { CreatedAt = -1 },
            joins: new UserJoinSpecification { IncludeRoles = true });

        token.ThrowIfCancellationRequested();
        
        _usersTotal = (int)response.Total;
        _pagedUsers = response.Items;

        return new TableData<OutUser>
        {
            TotalItems = _usersTotal,
            Items = _pagedUsers
        };
    }

    private async Task ApproveArtist(OutArtist artist)
    {
        var raw = artist.ToRaw();
        raw.IsVisible = true;
        await Api.SaveAsync(raw);
        await LoadArtistsSectionData(force: true);
        await LoadOverviewData(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task RejectArtist(OutArtist artist)
    {
        await Api.DeleteAsync<InArtist>(artist.Id);
        await LoadArtistsSectionData(force: true);
        await LoadOverviewData(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task ApproveGenre(OutGenre genre)
    {
        var raw = genre.ToRaw();
        raw.IsVisible = true;
        await Api.SaveAsync(raw);
        await LoadGenresSectionData(force: true);
        await LoadOverviewData(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task RejectGenre(OutGenre genre)
    {
        await Api.DeleteAsync<InGenre>(genre.Id);
        await LoadGenresSectionData(force: true);
        await LoadOverviewData(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task ToggleUserRole((OutUser User, OutRole Role, bool IsAssigned) payload)
    {
        var raw = payload.User.ToRaw();
        var roleIds = raw.RoleIds?.ToHashSet() ?? [];
        if (payload.IsAssigned)
        {
            roleIds.Remove(payload.Role.Id);
        }
        else
        {
            roleIds.Add(payload.Role.Id);
        }

        raw.RoleIds = roleIds.ToList();
        var response = await Api.SaveAsync(raw);
        if (!response.IsSuccessStatusCode)
        {
            Snackbar.Add("Could not update user roles.", Severity.Error);
            return;
        }

        Snackbar.Add("User roles updated.", Severity.Success);
        await LoadUsersSectionData(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return;
        }
        
        await Api.SaveAsync(new InRole { Name = roleName.Trim(), PermissionIds = [] });
        await LoadRolesAndPermissions(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddPermission(string permissionName)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
        {
            return;
        }
        
        await Api.SaveAsync(new InPermission { Name = permissionName.Trim() });
        await LoadRolesAndPermissions(force: true);
        await InvokeAsync(StateHasChanged);
    }

    private async Task SaveRole(OutRole role)
    {
        await Api.SaveAsync(role.ToRaw());
        Snackbar.Add($"Saved role: {role.Name}", Severity.Success);
        await LoadRolesAndPermissions(force: true);
        await InvokeAsync(StateHasChanged);
    }
}