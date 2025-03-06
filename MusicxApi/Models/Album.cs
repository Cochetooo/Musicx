using Musicx.Models.Enums;

namespace MusicxApi.Models;

public class Album
{
    public ulong Id { get; set; }
    
    public ulong? ArtistId { get; set; }
    public Dictionary<ulong, ArtistRole> CreditArtistIds { get; init; } = [];
    public ulong? LabelId { get; init; }
    public List<ulong> GenreIds { get; init; } = [];
    public List<ulong> InfluenceGenreIds { get; init; } = [];
    
    public string? ArtworkUrl { get; set; }
    public string? CatalogNumber { get; set; }
    public uint? DiscTotal { get; set; }
    public required string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public uint? TrackTotal { get; set; }
}