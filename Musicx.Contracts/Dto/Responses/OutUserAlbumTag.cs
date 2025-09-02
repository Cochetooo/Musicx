namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserAlbumTag : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutAlbum Album { get; set; } = null!;
    public OutTag Tag { get; set; } = null!;
}