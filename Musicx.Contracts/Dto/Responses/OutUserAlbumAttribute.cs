using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserAlbumAttribute : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutAlbum Album { get; set; } = null!;
    
    public CollectionType? CollectionType { get; set; }
    public short? Rating { get; set; }
    public string? Review { get; set; }
}