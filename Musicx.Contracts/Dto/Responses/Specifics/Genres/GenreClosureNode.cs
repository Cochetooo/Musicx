using Musicx.Contracts.Dto.Responses.Genre;

namespace Musicx.Contracts.Dto.Responses.Specifics.Genres;

public sealed class GenreClosureNode
{
    public OutGenre Relation { get; set; } = null!;
    public int Depth { get; set; }
}