using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InSong : BaseInputModel
{
    // Primary Key
    public long Id { get; set; }

        // Required Relationships
    public IReadOnlyList<long>? PrimaryGenreIds { get; set; } = [];
    public IReadOnlyList<long>? InfluenceGenreIds { get; set; } = [];

    // Required Columns
    public string Title { get; set; } = string.Empty;
    public SongType Type { get; set; }

    // Optional Relationships
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }

    // Optional Columns
    public int? DiscNumber { get; set; }
    public long? Duration { get; set; }
    public string? Lyrics { get; set; }
    public int? TrackNumber { get; set; }
}