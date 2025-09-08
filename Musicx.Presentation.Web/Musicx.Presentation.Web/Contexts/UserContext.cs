using System.Security.Claims;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Contexts;

public interface IUserContext
{
    OutUser? CurrentUser { get; }
    IReadOnlyCollection<OutRole>? Roles { get; }
    IReadOnlyCollection<OutPermission>? Permissions { get; }
    bool Can(string permission);
}

public sealed class UserContext(
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository) : IUserContext
{
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
        => Permissions.Any(p => p.Name.Equals(permission, StringComparison.OrdinalIgnoreCase));

    private void EnsureLoaded()
    {
        if (_cachedUser != null)
        {
            return;
        }
        
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return;
        }

        if (!long.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var user = userRepository.FindByIdAsync(userId, new UserQuerySpecification
        {
            IncludeRoles = true
        }).Result;

        if (user is null)
        {
            return;
        }
        
        _cachedUser = user;
        _cachedRoles = user.Roles!.ToList();
        _cachedPermissions = user.Roles!
            .SelectMany(r => r.Permissions!)
            .DistinctBy(p => p.Id)
            .ToList();
    }
}