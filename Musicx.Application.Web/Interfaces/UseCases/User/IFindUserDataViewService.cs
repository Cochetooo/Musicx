using Musicx.Contracts.Dto.Responses.Specifics.Users;

namespace Musicx.Application.Web.Interfaces.UseCases.User;

/// <summary>
/// Retrieves the consolidated User DataView for Web clients.
/// </summary>
/// <since>0.7.4</since>
public interface IFindUserDataViewService
{
    /// <summary>
    /// Loads user-centric data optimized for the User page.
    /// </summary>
    /// <param name="userId">Target user identifier.</param>
    /// <param name="currentUserId">Optional viewer id used for permission-scoped data.</param>
    Task<OutUserDataView?> ExecuteAsync(long userId, long? currentUserId = null);
}