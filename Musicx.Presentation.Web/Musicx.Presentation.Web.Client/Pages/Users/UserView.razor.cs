using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserView
{
    private ILogger _logger = null!;

    private MudTable<OutUserAlbumAttribute> _albumRatingsTable = null!;
    
    private OutUser? _user;

    private long _albumCount;
    private string _searchString = string.Empty;

    private OutUserRatingStats? _albumRatingDistrib;
    
    [Parameter] public string? Id { get; set; }
    
    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(nameof(UserView));
    }

    protected override async Task OnParametersSetAsync()
    {
        await Load();
    }

    private async Task Load()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Album ID is null or empty.");
            return;
        }
        
        if (!long.TryParse(Id, out var userId))
        {
            _logger.LogError("❌ Invalid User ID format: {Id}", Id);
            return;
        }

        _user = await UcGet.ExecuteAsync(userId, "role");

        if (_user == null)
        {
            _logger.LogError("❌ User has not been found.");
            return;
        }

        _logger.LogInformation($"✅ User loaded: {_user.Name} ({_user.Id})");
        await InvokeAsync(StateHasChanged);

        _albumRatingDistrib = await UcGetAlbumRatingDistrib.ExecuteAsync(_user.Id);
        _logger.LogInformation($"✅ Album Ratings Distribution loaded ({_user.Id})");
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task<TableData<OutUserAlbumAttribute>> LoadUserAttrData(TableState state, CancellationToken token)
    {
        if (_user is null)
        {
            _logger.LogWarning("⚠️ User ID is null, cannot load user attributes data.");
            return new TableData<OutUserAlbumAttribute>
            {
                TotalItems = 0,
                Items = []
            };
        }
        
        var response = await UcGetAlbumAttrs.ExecuteAsync(
            _user.Id, 
            query: "artist",
            skip: state.Page * state.PageSize,
            take: state.PageSize,
            filter: string.IsNullOrWhiteSpace(_searchString) ? null : _searchString,
            token: token
        );

        _albumCount = response.Total;
        await InvokeAsync(StateHasChanged);

        return new TableData<OutUserAlbumAttribute>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _albumRatingsTable.ReloadServerData();
    }
}