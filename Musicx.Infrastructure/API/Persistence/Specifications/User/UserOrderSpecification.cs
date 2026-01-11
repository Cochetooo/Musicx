using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.User;

namespace Musicx.Infrastructure.API.Persistence.Specifications.User;

public sealed record UserOrderSpecification : OrderSpecification<InUser>
{
    public short? BirthDate { get; init; }
    public short? Email { get; init; }
    public short? Name { get; init; }
    
    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"u0.{UserColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"u0.{UserColumns.UpdatedAt}", dir),
            nameof(BirthDate) => new($"u0.{UserColumns.BirthDate}", dir),
            nameof(Email) => new($"u0.{UserColumns.Email}", dir),
            nameof(Name) => new($"u0.{UserColumns.Name}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}