namespace Musicx.Contracts.Dto.Responses.Specifics.Genres;

public sealed class GenreRelationNode
{
    public OutGenreRelation Relation { get; set; } = null!;
    public OutGenre RelatedGenre { get; set; } = null!;
}