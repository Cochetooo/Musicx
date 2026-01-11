using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.User;

namespace Musicx.Infrastructure.API.Persistence.Specifications.User;

public sealed record UserAlbumAttrOrderSpecification : OrderSpecification<InUserAlbumAttribute>
{
    public short? AlbumName { get; init; }
    public short? AlbumOriginalReleaseDate { get; init; }
    public short? ArtistName { get; init; }
    public short? CollectionType { get; init; }
    public short? DiscoveryDate { get; init; }
    public short? Rating { get; init; }
    
    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"uaa0.{UserAlbumAttrColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"uaa0.{UserAlbumAttrColumns.UpdatedAt}", dir),
            nameof(AlbumName) => new($"al0.{AlbumColumns.Name}", dir),
            nameof(AlbumOriginalReleaseDate) => new($"al0.{AlbumColumns.OriginalReleaseDate}", dir),
            nameof(ArtistName) => new($"ar0.{ArtistColumns.Name}", dir),
            nameof(CollectionType) => new($"uaa0.{UserAlbumAttrColumns.CollectionType}", dir),
            nameof(DiscoveryDate) => new($"uaa0.{UserAlbumAttrColumns.DiscoveryDate}", dir),
            nameof(Rating) => new($"uaa0.{UserAlbumAttrColumns.Rating}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}