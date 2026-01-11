using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Infrastructure.API.Persistence.Columns.Song;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Song;

public sealed record SongOrderSpecification : OrderSpecification<InSong>
{
    public short? DiscNumber { get; init; }
    public short? Duration { get; init; }
    public short? IsVisible { get; init; }
    public short? Title { get; init; }
    public short? TrackNumber { get; init; }
    public short? Type { get; init; }
    
    public override void Validate()
    {
        
    }

    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"s0.{SongColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"s0.{SongColumns.UpdatedAt}", dir),
            nameof(DiscNumber) => new($"s0.{SongColumns.DiscNumber}", dir),
            nameof(Duration) => new($"s0.{SongColumns.Duration}", dir),
            nameof(IsVisible) => new($"s0.{SongColumns.IsVisible}", dir),
            nameof(Title) => new($"s0.{SongColumns.Title}", dir),
            nameof(TrackNumber) => new($"s0.{SongColumns.TrackNumber}", dir),
            nameof(Type) => new($"s0.{SongColumns.Type}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}