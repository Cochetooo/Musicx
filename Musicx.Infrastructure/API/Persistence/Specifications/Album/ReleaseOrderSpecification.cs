using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Album;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Album;

public sealed record ReleaseOrderSpecification : OrderSpecification<InRelease>
{
    public short? CatalogNumber { get; init; }
    public short? IsVisible { get; init; }
    public short? ReleaseDate { get; init; }

    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"rl0.{ReleaseColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"rl0.{ReleaseColumns.UpdatedAt}", dir),
            nameof(CatalogNumber) => new($"rl0.{ReleaseColumns.CatalogNumber}", dir),
            nameof(IsVisible) => new($"rl0.{ReleaseColumns.IsVisible}", dir),
            nameof(ReleaseDate) => new($"rl0.{ReleaseColumns.ReleaseDate}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}