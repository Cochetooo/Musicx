using Musicx.Models.Enums;

namespace MusicxApi.Models;

public class Album
{
    public ulong Id { get; set; }
    
    public ulong? ArtistId { get; set; }
    public Dictionary<ulong, ArtistRole> CreditArtistIds { get; } = [];
    public ulong? LabelId { get; set; }
    public List<ulong> GenreIds { get; } = [];
    public List<ulong> InfluenceGenreIds { get; } = [];
    
    public string? ArtworkUrl { get; set; }
    public string? CatalogNumber { get; set; }
    public uint? DiscTotal { get; set; }
    public required string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public uint? TrackTotal { get; set; }
}