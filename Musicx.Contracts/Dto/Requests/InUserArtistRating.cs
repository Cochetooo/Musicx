namespace Musicx.Contracts.Dto.Requests;

public sealed class InUserArtistRating : BaseInputModel
{
    public long UserId { get; set; }
    public long ArtistId { get; set; }
    
    public short Rating { get; set; }
}