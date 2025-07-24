namespace Musicx.Domain.Models;

public sealed class AlbumGenre
{
    public long AlbumId { get; set; }
    public Album Album { get; set; }
    
    public long GenreId { get; set; }
    public Genre Genre { get; set; }
    
    public int Level { get; set; }
}