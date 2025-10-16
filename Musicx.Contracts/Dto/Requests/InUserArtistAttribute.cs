namespace Musicx.Contracts.Dto.Requests;

public sealed class InUserArtistAttribute : BaseInputModel
{
    public long UserId { get; set; }
    public long ArtistId { get; set; }
    
    public bool? Follow { get; set; }
    public short? Rating { get; set; }
}