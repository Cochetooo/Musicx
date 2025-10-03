using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserView
{
    private ILogger _logger = null!;
    
    private OutUser? _user { get; set; }
    private List<OutUserAlbumAttribute> _albumAttrs { get; set; } = [];
    
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
        
        _albumAttrs = await UcGetAlbumAttrs.ExecuteAsync(userId);
        
        _logger.LogInformation($"✅ User album attributes loaded");
        await InvokeAsync(StateHasChanged);
    }
}