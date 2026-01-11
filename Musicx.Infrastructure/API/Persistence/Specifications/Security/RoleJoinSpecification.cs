using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Security;

public sealed class RoleJoinSpecification : IJoinSpecification<InRole>
{
    /// <summary>
    /// Include the permissions of this role.
    /// </summary>
    /// <since>0.6.5</since>
    public bool IncludePermissions { get; set; }
}