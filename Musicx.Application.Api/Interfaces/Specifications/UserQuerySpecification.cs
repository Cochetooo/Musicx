using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Api.Interfaces.Specifications;

/// <summary>
/// Specify the relation to include when retrieving user from repository.
/// </summary>
/// <since>0.6.5</since>
public sealed class UserQuerySpecification : IQuerySpecification<InUser>
{
    /// <summary>
    /// Include the roles of this user.
    /// </summary>
    /// <since>0.6.5</since>
    public bool IncludeRoles { get; set; }
}