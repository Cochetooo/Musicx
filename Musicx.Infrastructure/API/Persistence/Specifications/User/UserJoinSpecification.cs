using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;

namespace Musicx.Infrastructure.API.Persistence.Specifications.User;

/// <summary>
/// Specify the relation to include when retrieving user from repository.
/// </summary>
/// <since>0.6.5</since>
public sealed class UserJoinSpecification : IJoinSpecification<InUser>
{
    /// <summary>
    /// Include the roles of this user.
    /// </summary>
    /// <since>0.6.5</since>
    public bool IncludeRoles { get; set; }
}