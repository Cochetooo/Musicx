namespace Musicx.Contracts.Dto.Responses.Specifics.Artists;

public sealed class OutArtistAlbumSummary
{
    public long ArtistId { get; set; }
    public OutArtist Artist { get; set; } = null!;
    public decimal? Rating { get; set; }
    public long RatingsCount { get; set; }
}