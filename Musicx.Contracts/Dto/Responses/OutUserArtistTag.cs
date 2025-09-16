namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserArtistTag : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutArtist Artist { get; set; } = null!;
    public OutTag Tag { get; set; } = null!;
    
    public long UserId { get; set; }
    public long ArtistId { get; set; }
    public long TagId { get; set; }
}