using System.Security.Claims;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Specifications.Security;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

namespace Musicx.Presentation.Web.Contexts;

public interface IUserContext
{
    OutUser? CurrentUser { get; }
    IReadOnlyCollection<OutRole>? Roles { get; }
    IReadOnlyCollection<OutPermission>? Permissions { get; }
    bool Can(string permission);
}

public sealed class UserContext(
    ILoggerProvider loggerProvider,
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository,
    IRoleRepository roleRepository) : IUserContext
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserContext));
    
    private OutUser? _cachedUser;
    private IReadOnlyCollection<OutRole>? _cachedRoles;
    private IReadOnlyCollection<OutPermission>? _cachedPermissions;

    public OutUser? CurrentUser
    {
        get
        {
            EnsureLoaded();
            return _cachedUser;
        }
    }
    
    public IReadOnlyCollection<OutRole> Roles
    {
        get
        {
            EnsureLoaded();
            return _cachedRoles ?? [];
        }
    }
    
    public IReadOnlyCollection<OutPermission> Permissions
    {
        get
        {
            EnsureLoaded();
            return _cachedPermissions ?? [];
        }
    }

    public bool Can(string permission)
        => Permissions.Any(p => p.Name.Equals(permission, StringComparison.OrdinalIgnoreCase))
        || Roles.Any(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));

    private void EnsureLoaded()
    {
        _logger.LogDebug("🔄️ Loading User context Data...");
        
        if (_cachedUser != null)
        {
            return;
        }

        try
        {
            var userIdClaim = httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return;
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                return;
            }

            var user = userRepository.FindOneByIdAsync(userId, new UserJoinSpecification
            {
                IncludeRoles = true
            }).Result;

            if (user is null)
            {
                return;
            }

            _cachedUser = user;
            _cachedRoles = user.Roles!.ToList();
            _cachedPermissions = [];
            
            List<OutPermission> foundPermissions = [];
            
            foreach (var role in _cachedRoles)
            {
                var permissions = roleRepository.FindOneByIdAsync(role.Id,
                    new RoleJoinSpecification
                    {
                        IncludePermissions = true
                    }).Result?.Permissions;

                if (permissions is not null)
                {
                    foundPermissions.AddRange(permissions);
                }
            }

            _cachedPermissions = foundPermissions;
            
            _logger.LogDebug($"✅ Data Context loaded successfully ! Roles: {string.Join(", ", _cachedRoles.Select(r => r.Name))}" );
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error while loading user context: {ex.Message}");
        }
    }
}