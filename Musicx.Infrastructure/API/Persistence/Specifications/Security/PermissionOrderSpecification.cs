using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Infrastructure.API.Persistence.Columns.Security;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Security;

public sealed record PermissionOrderSpecification : OrderSpecification<InPermission>
{
    public short? Name { get; init; }
    
    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"p0.{PermissionColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"p0.{PermissionColumns.UpdatedAt}", dir),
            nameof(Name) => new($"p0.{PermissionColumns.Name}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}