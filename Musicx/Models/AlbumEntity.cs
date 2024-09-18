using Musicx.Models.Enums;

namespace Musicx.Models;

public class AlbumEntity
{
    public long Id { get; set; }
    
    public long? ArtistId { get; set; }
    public Dictionary<long, ArtistRole> CreditArtistIds { get; } = new();
    public long? LabelId { get; set; }
    public List<long> GenreIds { get; } = new();
    public List<long> InfluenceGenreIds { get; } = new();
    
    public string? ArtworkUrl { get; set; }
    public string? CatalogNumber { get; set; }
    public short? DiscTotal { get; set; }
    public required string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public short? TrackTotal { get; set; }
    public ReleaseType? ReleaseType { get; set; }
}