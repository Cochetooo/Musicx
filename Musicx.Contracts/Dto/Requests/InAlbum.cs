namespace Musicx.Contracts.Dto.Requests;

public sealed class InAlbum
{
    // Primary Key
    public long Id { get; set; }
    
    // Required Relationships
    public IReadOnlyList<long> ReleaseIds { get; set; } = [];
    public IReadOnlyList<long> PrimaryGenreIds { get; set; } = [];
    public IReadOnlyList<long> InfluenceGenreIds { get; set; } = [];
    
    // Required Columns
    public string Name { get; set; } = string.Empty;

    // Optional Relationships
    public long? ArtistId { get; set; }
    
    // Optional Columns
    public string? ArtworkUrl { get; set; } = string.Empty;
    public int? DiscTotal { get; set; }
    public DateTime? ReleaseDate { get; set; } = DateTime.MinValue;
    public string? ReleaseType { get; set; } = string.Empty;
    public int? TrackTotal { get; set; }
}