using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InUserAlbumAttribute : BaseInputModel
{
    public long UserId { get; set; }
    public long AlbumId { get; set; }
    
    public short? Rating { get; set; }
    public CollectionType? CollectionType { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public string? Review { get; set; }
}