using Musicx.Contracts.Dto.Responses.Genre;

namespace Musicx.Contracts.Dto.Responses.Specifics.Artists;

public sealed class OutArtistGenreStat
{
    public OutGenre Genre { get; set; } = null!;
    public int AlbumCount { get; set; }
}