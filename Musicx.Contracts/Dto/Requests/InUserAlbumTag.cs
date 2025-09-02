namespace Musicx.Contracts.Dto.Requests;

public sealed class InUserAlbumTag
{
    public long UserId { get; set; }
    public long AlbumId { get; set; }
    public long TagId { get; set; }
}