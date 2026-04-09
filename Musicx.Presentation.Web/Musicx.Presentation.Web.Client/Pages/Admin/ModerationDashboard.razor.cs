using MudBlazor;
using Musicx.Application.Shared.Enums;
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

    private ModerationSection _section = ModerationSection.Overview;

    private ArtistEditModal _artistEditModal = null!;
    private GenreEditModal _genreEditModal = null!;

    private List<OutArtist> _artists = [];
    private List<OutGenre> _genres = [];
    private List<OutAlbum> _albums = [];
    private List<OutUser> _users = [];
    private List<OutRole> _roles = [];
    private List<OutPermission> _permissions = [];

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

    private int _genresToday;
    private int _genresWeek;
    private int _genresMonth;
    private int _genresYear;
    private int _genresVisible;
    private int _genresHidden;

    private int _albumsToday;
    private int _albumsVisible;
    private int _albumsHidden;

    private int _usersToday;

    private bool IsAdmin => UserClientContext.Roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                            || UserClientContext.Can("admin.role.read");

    private IReadOnlyList<OutArtist> PendingArtists => _artists
        .Where(x => !x.IsVisible)
        .OrderByDescending(x => x.CreatedAt)
        .Take(25)
        .ToList();

    private IReadOnlyList<OutGenre> PendingGenres => _genres
        .Where(x => !x.IsVisible)
        .OrderByDescending(x => x.CreatedAt)
        .Take(25)
        .ToList();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await LoadData();
        await InvokeAsync(StateHasChanged);
    }

    private void SetSection(ModerationSection section) => _section = section;

    private async Task LoadData()
    {
        var artistsTask = Api.FindAsync<InArtist, OutArtist>(pagingOptions: new PagingOptions(10_000, 0), order: new ArtistOrderSpecification { CreatedAt = -1 });
        var genresTask = Api.FindAsync<InGenre, OutGenre>(pagingOptions: new PagingOptions(10_000, 0), order: new GenreOrderSpecification { CreatedAt = -1 });
        var albumsTask = Api.FindAsync<InAlbum, OutAlbum>(pagingOptions: new PagingOptions(10_000, 0), order: new AlbumOrderSpecification { CreatedAt = -1 }, joins: new AlbumJoinSpecification { IncludeArtist = true });
        var usersTask = Api.FindAsync<InUser, OutUser>(pagingOptions: new PagingOptions(10_000, 0), order: new UserOrderSpecification { CreatedAt = -1 }, joins: new UserJoinSpecification { IncludeRoles = true });
        var rolesTask = Api.FindAsync<InRole, OutRole>(pagingOptions: new PagingOptions(10_000, 0), joins: new RoleJoinSpecification { IncludePermissions = true });
        var permissionsTask = Api.FindAsync<InPermission, OutPermission>(pagingOptions: new PagingOptions(10_000, 0));

        await Task.WhenAll(artistsTask, genresTask, albumsTask, usersTask, rolesTask, permissionsTask);

        _artists = artistsTask.Result.Items;
        _genres = genresTask.Result.Items;
        _albums = albumsTask.Result.Items;
        _users = usersTask.Result.Items;
        _roles = rolesTask.Result.Items;
        _permissions = permissionsTask.Result.Items;

        ComputeStats();
    }

    private void ComputeStats()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var yearStart = new DateTime(now.Year, 1, 1);

        _artistsToday = _artists.Count(a => a.CreatedAt >= today);
        _artistsWeek = _artists.Count(a => a.CreatedAt >= weekStart);
        _artistsMonth = _artists.Count(a => a.CreatedAt >= monthStart);
        _artistsYear = _artists.Count(a => a.CreatedAt >= yearStart);
        _artistsVisible = _artists.Count(a => a.IsVisible);
        _artistsHidden = _artists.Count - _artistsVisible;

        _genresToday = _genres.Count(g => g.CreatedAt >= today);
        _genresWeek = _genres.Count(g => g.CreatedAt >= weekStart);
        _genresMonth = _genres.Count(g => g.CreatedAt >= monthStart);
        _genresYear = _genres.Count(g => g.CreatedAt >= yearStart);
        _genresVisible = _genres.Count(g => g.IsVisible);
        _genresHidden = _genres.Count - _genresVisible;

        _albumsToday = _albums.Count(a => a.CreatedAt >= today);
        _albumsVisible = _albums.Count(a => a.IsVisible);
        _albumsHidden = _albums.Count - _albumsVisible;

        _usersToday = _users.Count(a => a.CreatedAt >= today);
    }

    private async Task<TableData<OutArtist>> LoadArtists(TableState state, CancellationToken token)
    {
        await Task.Delay(1, token);
        var query = _artists.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(_artistSearch))
        {
            query = query.Where(a => a.Name.Contains(_artistSearch, StringComparison.OrdinalIgnoreCase));
        }

        var rows = query.OrderByDescending(a => a.CreatedAt).ToList();
        return new TableData<OutArtist>
        {
            TotalItems = rows.Count,
            Items = rows.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList()
        };
    }

    private async Task<TableData<OutGenre>> LoadGenres(TableState state, CancellationToken token)
    {
        await Task.Delay(1, token);
        var query = _genres.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(_genreSearch))
        {
            query = query.Where(g => g.CanonicalName.Contains(_genreSearch, StringComparison.OrdinalIgnoreCase));
        }

        var rows = query.OrderByDescending(g => g.CreatedAt).ToList();
        return new TableData<OutGenre>
        {
            TotalItems = rows.Count,
            Items = rows.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList()
        };
    }

    private async Task<TableData<OutAlbum>> LoadAlbums(TableState state, CancellationToken token)
    {
        await Task.Delay(1, token);
        var query = _albums.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(_albumSearch))
        {
            query = query.Where(a => a.Name.Contains(_albumSearch, StringComparison.OrdinalIgnoreCase));
        }

        var rows = query.OrderByDescending(a => a.CreatedAt).ToList();
        return new TableData<OutAlbum>
        {
            TotalItems = rows.Count,
            Items = rows.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList()
        };
    }

    private async Task<TableData<OutUser>> LoadUsers(TableState state, CancellationToken token)
    {
        await Task.Delay(1, token);
        var query = _users.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(_userSearch))
        {
            query = query.Where(u =>
                u.Name.Contains(_userSearch, StringComparison.OrdinalIgnoreCase)
                || u.Email.Contains(_userSearch, StringComparison.OrdinalIgnoreCase));
        }

        var rows = query.OrderByDescending(u => u.CreatedAt).ToList();
        _usersTotal = rows.Count;
        _pagedUsers = rows.Skip(state.Page * state.PageSize).Take(state.PageSize).ToList();

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
        await LoadData();
    }

    private async Task RejectArtist(OutArtist artist)
    {
        await Api.DeleteAsync<InArtist>(artist.Id);
        await LoadData();
    }

    private async Task ApproveGenre(OutGenre genre)
    {
        var raw = genre.ToRaw();
        raw.IsVisible = true;
        await Api.SaveAsync(raw);
        await LoadData();
    }

    private async Task RejectGenre(OutGenre genre)
    {
        await Api.DeleteAsync<InGenre>(genre.Id);
        await LoadData();
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
        await LoadData();
    }

    private async Task AddRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName)) return;
        await Api.SaveAsync(new InRole { Name = roleName.Trim(), PermissionIds = [] });
        await LoadData();
    }

    private async Task AddPermission(string permissionName)
    {
        if (string.IsNullOrWhiteSpace(permissionName)) return;
        await Api.SaveAsync(new InPermission { Name = permissionName.Trim() });
        await LoadData();
    }

    private async Task SaveRole(OutRole role)
    {
        await Api.SaveAsync(role.ToRaw());
        Snackbar.Add($"Saved role: {role.Name}", Severity.Success);
        await LoadData();
    }
}