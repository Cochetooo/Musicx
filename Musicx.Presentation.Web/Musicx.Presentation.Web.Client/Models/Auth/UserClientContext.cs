using System.Net.Http.Json;
using Musicx.Application.Web.Interfaces.Models.Auth;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Presentation.Web.Client.Models.Auth;

public sealed class UserClientContext(
    ILoggerFactory loggerFactory,
    HttpClient httpClient) : IUserClientContext
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UserClientContext>();
    
    private OutUser? _currentUser;
    private List<OutRole> _roles = [];
    private List<OutPermission> _permissions = [];
    
    public OutUser? CurrentUser => _currentUser;
    public IReadOnlyCollection<OutRole> Roles => _roles;
    public IReadOnlyCollection<OutPermission> Permissions => _permissions;
    
    public bool Can(string permission)
        => _permissions.Any(p => p.Name.Equals(permission, StringComparison.OrdinalIgnoreCase))
        || _roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));

    public async Task RefreshAsync()
    {
        _logger.LogInformation("🔄️ Refreshing User Context...");
        
        try
        {
            var response = await httpClient.GetFromJsonAsync<UserMeResponse>("/api/auth/me");
            if (response is not null)
            {
                _currentUser = response.CurrentUser;
                _roles = response.Roles.ToList();
                _permissions = response.Permissions.ToList();
                
                _logger.LogInformation("✅ User context loaded successfully !");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error while loading User context : {ex.Message}");
            _currentUser = null;
            _roles.Clear();
            _permissions.Clear();
        }
    }
}

public record UserMeResponse(
    OutUser CurrentUser,
    List<OutRole> Roles,
    List<OutPermission> Permissions
);