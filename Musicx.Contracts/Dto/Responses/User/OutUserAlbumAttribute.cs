using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses.User;

public sealed class OutUserAlbumAttribute : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutAlbum Album { get; set; } = null!;
    
    public CollectionType? CollectionType { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public short? Rating { get; set; }
    public short? ProductionRating { get; set; }
    public short? LyricsRating { get; set; }
    public short? InstrumentationRating { get; set; }
    public short? VocalsRating { get; set; }
    public short? AtmosphereRating { get; set; }
    public short? OriginalityRating { get; set; }
    public string? Review { get; set; }
}