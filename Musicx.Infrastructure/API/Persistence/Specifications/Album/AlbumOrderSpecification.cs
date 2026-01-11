using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.Album;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Album;

public sealed record AlbumOrderSpecification : OrderSpecification<InAlbum>
{
    public short? ArtistAlias { get; init; }
    public short? BeginRecordDate { get; init; }
    public short? DiscTotal { get; init; }
    public short? EndRecordDate { get; init; }
    public short? EnglishName { get; init; }
    public short? IsExplicitContent { get; init; }
    public short? IsFarRight { get; init; }
    public short? IsGraphicContent { get; init; }
    public short? Language { get; init; }
    public short? Name { get; init; }
    public short? OriginalReleaseDate { get; init; }
    public short? ReleaseType { get; init; }
    public short? TrackTotal { get; init; }

    public override void Validate()
    {
        
    }

    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"al0.{AlbumColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"al0.{AlbumColumns.UpdatedAt}", dir),
            nameof(ArtistAlias) => new($"al0.{AlbumColumns.ArtistAlias}", dir),
            nameof(BeginRecordDate) => new($"al0.{AlbumColumns.BeginRecordDate}", dir),
            nameof(DiscTotal) => new($"al0.{AlbumColumns.DiscTotal}", dir),
            nameof(EndRecordDate) => new($"al0.{AlbumColumns.EndRecordDate}", dir),
            nameof(EnglishName) => new($"al0.{AlbumColumns.EnglishName}", dir),
            nameof(IsExplicitContent) => new($"al0.{AlbumColumns.IsExplicitContent}", dir),
            nameof(IsFarRight) => new($"al0.{AlbumColumns.IsFarRight}", dir),
            nameof(IsGraphicContent) => new($"al0.{AlbumColumns.IsGraphicContent}", dir),
            nameof(Language) => new($"al0.{AlbumColumns.Language}", dir),
            nameof(Name) => new($"al0.{AlbumColumns.Name}", dir),
            nameof(OriginalReleaseDate) => new($"al0.{AlbumColumns.OriginalReleaseDate}", dir),
            nameof(ReleaseType) => new($"al0.{AlbumColumns.ReleaseType}", dir),
            nameof(TrackTotal) => new($"al0.{AlbumColumns.TrackTotal}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}