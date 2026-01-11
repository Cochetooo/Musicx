using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests.Album;

public sealed class InAlbumGenre : BaseInputModel
{
    // Required Relationships
    public long AlbumId { get; set; }
    public long GenreId { get; set; }
    public long TaggerId { get; set; }
    
    // Required Columns
    public float Confidence { get; set; } = 0.8f;
    public GenreVoteSource Source { get; set; }
    
    // Optional Columns
    public string? Metadata { get; set; }
}