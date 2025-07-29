using Musicx.Domain.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InAlbum : BaseInputModel
{
    // Required Relationships
    public IReadOnlyList<long>? ReleaseIds { get; set; }
    public IReadOnlyList<long>? PrimaryGenreIds { get; set; }
    public IReadOnlyList<long>? InfluenceGenreIds { get; set; }
    
    // Required Columns
    public string Name { get; set; } = string.Empty;

    // Optional Relationships
    public long? ArtistId { get; set; }
    
    // Optional Columns
    public string? ArtworkUrl { get; set; }
    public DateTime? BeginRecordDate { get; set; }
    public int? DiscTotal { get; set; }
    public DateTime? EndRecordDate { get; set; }
    public bool IsFarRight { get; set; }
    public string? Language { get; set; }
    public DateTime? OriginalReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public int? TrackTotal { get; set; }
}