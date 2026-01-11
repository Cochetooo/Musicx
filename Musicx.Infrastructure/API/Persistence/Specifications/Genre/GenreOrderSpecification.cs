using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Genre;

public sealed record GenreOrderSpecification : OrderSpecification<InGenre>
{
    public short? CanonicalName { get; init; }
    public short? Color { get; init; }
    public short? Confidence { get; init; }
    public short? EraStart { get; init; }
    public short? EraEnd { get; init; }
    public short? IsVisible { get; init; }
    public short? IsTaggable { get; init; }
    public short? ShortName { get; init; }
    public short? Type { get; init; }
    
    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"g0.{GenreColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"g0.{GenreColumns.UpdatedAt}", dir),
            nameof(CanonicalName) => new($"g0.{GenreColumns.CanonicalName}", dir),
            nameof(Color) => new($"g0.{GenreColumns.Color}", dir),
            nameof(Confidence) => new($"g0.{GenreColumns.Confidence}", dir),
            nameof(EraStart) => new($"g0.{GenreColumns.EraStart}", dir),
            nameof(EraEnd) => new($"g0.{GenreColumns.EraEnd}", dir),
            nameof(IsVisible) => new($"g0.{GenreColumns.IsVisible}", dir),
            nameof(IsTaggable) => new($"g0.{GenreColumns.Taggable}", dir),
            nameof(ShortName) => new($"g0.{GenreColumns.ShortName}", dir),
            nameof(Type) => new($"g0.{GenreColumns.Type}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}