using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutAlbum : BaseOutputModel
{
    public OutArtist? Artist { get; set; }
    public ICollection<OutRelease>? Releases { get; set; }
    public ICollection<OutGenre>? PrimaryGenres { get; set; }
    public ICollection<OutGenre>? InfluenceGenres { get; set; }
    public OutAlbumRatingStat? Stats { get; set; }
    
    public long? ArtistId { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public DateTime? BeginRecordDate { get; set; }
    public int? DiscTotal { get; set; }
    public DateTime? EndRecordDate { get; set; }
    public bool IsFarRight { get; set; }
    public bool IsNsfw { get; set; }
    public string? Language { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? OriginalReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public string? SimplifiedGenreColor { get; set; }
    public string? SimplifiedGenreName { get; set; }
    public long? TotalDuration { get; set; }
    public int? TrackTotal { get; set; }
}