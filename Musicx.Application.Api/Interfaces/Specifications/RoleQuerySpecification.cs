using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Api.Interfaces.Specifications;

public sealed class RoleQuerySpecification : IQuerySpecification<InRole>
{
    /// <summary>
    /// Include the permissions of this role.
    /// </summary>
    /// <since>0.6.5</since>
    public bool IncludePermissions { get; set; }
}