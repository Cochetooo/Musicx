namespace Musicx.Application.Desktop.Models;

public sealed class EncyclopediaTrackMatch
{
    public bool Found { get; set; }
    
    public long? ApiAlbumId { get; set; }
    public long? ApiArtistId { get; set; }
    public long? ApiSongId { get; set; }
    public string? ApiTrackId { get; set; }
    public string? CanonicalTitle { get; set; }
    public string? CanonicalArtist { get; set; }
    public string? CanonicalAlbum { get; set; }
}