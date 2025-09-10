using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.Models.Auth;

public interface IUserClientContext
{
    OutUser? CurrentUser { get; }
    IReadOnlyCollection<OutRole> Roles { get; }
    IReadOnlyCollection<OutPermission> Permissions { get; }
    bool Can(string permission);
    Task RefreshAsync();
}