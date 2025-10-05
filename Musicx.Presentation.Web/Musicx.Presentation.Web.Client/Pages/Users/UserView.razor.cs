using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserView
{
    private ILogger _logger = null!;

    private OutUser? _user;

    private long _albumCount;
    
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
            skip: state.Page * state.PageSize,
            take: state.PageSize,
            token
        );

        _albumCount = response.Total;
        await InvokeAsync(StateHasChanged);

        return new TableData<OutUserAlbumAttribute>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }
}