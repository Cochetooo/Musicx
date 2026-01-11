namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserAlbumTag : BaseInputModel
{
    public long UserId { get; set; }
    public long AlbumId { get; set; }
    public long TagId { get; set; }
}