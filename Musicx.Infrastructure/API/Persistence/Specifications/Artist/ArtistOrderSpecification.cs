using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Artist;

public sealed record ArtistOrderSpecification : OrderSpecification<InArtist>
{
    public short? Alias { get; init; }
    public short? CurrentCountry { get; init; }
    public short? CurrentRegion { get; init; }
    public short? CurrentTown { get; init; }
    public short? IsVisible { get; init; }
    public short? Name { get; init; }
    public short? OriginCountry { get; init; }
    public short? OriginRegion { get; init; }
    public short? OriginTown { get; init; }

    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"ar0.{ArtistColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"ar0.{ArtistColumns.UpdatedAt}", dir),
            nameof(Alias) => new($"ar0.{ArtistColumns.Alias}", dir),
            nameof(CurrentCountry) => new($"ar0.{ArtistColumns.CurrentCountry}", dir),
            nameof(CurrentRegion) => new($"ar0.{ArtistColumns.CurrentRegion}", dir),
            nameof(CurrentTown) => new($"ar0.{ArtistColumns.CurrentTown}", dir),
            nameof(IsVisible) => new($"ar0.{ArtistColumns.IsVisible}", dir),
            nameof(Name) => new($"ar0.{ArtistColumns.Name}", dir),
            nameof(OriginCountry) => new($"ar0.{ArtistColumns.OriginCountry}", dir),
            nameof(OriginRegion) => new($"ar0.{ArtistColumns.OriginRegion}", dir),
            nameof(OriginTown) => new($"ar0.{ArtistColumns.OriginTown}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}