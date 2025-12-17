namespace Musicx.Contracts.Dto.Responses.Specifics.Genres;

public sealed class AlbumInfluenceNode
{
    public OutAlbumInfluence Relation { get; set; } = null!;
    public OutGenre Genre { get; set; } = null!;
}