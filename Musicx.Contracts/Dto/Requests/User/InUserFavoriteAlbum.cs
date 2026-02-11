namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserFavoriteAlbum : BaseInputModel
{
    public long UserId { get; set; }
    public long AlbumId { get; set; }
    
    public string? Note { get; set; }
    public short Order { get; set; }
}