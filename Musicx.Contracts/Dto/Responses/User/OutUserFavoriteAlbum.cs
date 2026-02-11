namespace Musicx.Contracts.Dto.Responses.User;

public sealed class OutUserFavoriteAlbum : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutAlbum Album { get; set; } = null!;
    
    public string? Note { get; set; }
    public short Order { get; set; }
}