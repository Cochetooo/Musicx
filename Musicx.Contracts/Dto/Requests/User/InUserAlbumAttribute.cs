using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserAlbumAttribute : BaseInputModel
{
    public long UserId { get; set; }
    public long AlbumId { get; set; }
    
    public short? Rating { get; set; }
    public short? ProductionRating { get; set; }
    public short? LyricsRating { get; set; }
    public short? InstrumentationRating { get; set; }
    public short? VocalsRating { get; set; }
    public short? AtmosphereRating { get; set; }
    public short? OriginalityRating { get; set; }
    public CollectionType? CollectionType { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public string? Review { get; set; }
    public DateTime? ReviewPostedAt { get; set; }
    public long? ReviewSourceId { get; set; }
}