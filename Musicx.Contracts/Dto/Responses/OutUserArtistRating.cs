namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserArtistRating : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutArtist Artist { get; set; } = null!;
    
    public long UserId { get; set; }
    public long ArtistId { get; set; }
    
    public short Rating { get; set; }
}