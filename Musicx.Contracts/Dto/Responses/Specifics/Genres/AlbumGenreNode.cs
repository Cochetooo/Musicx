namespace Musicx.Contracts.Dto.Responses.Specifics.Genres;

public sealed class AlbumGenreNode
{
    public OutAlbumGenre Relation { get; set; } = null!;
    public OutGenre Genre { get; set; } = null!;
}