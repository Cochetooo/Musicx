using Musicx.Models.Enums;

namespace Musicx.Models;

public class ArtistEntity
{
    public long Id { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public Country? Country { get; set; }
    public required string Name { get; set; }
}