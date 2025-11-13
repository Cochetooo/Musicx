namespace Musicx.Contracts.Dto.Requests;

public sealed class InEventArtist : BaseInputModel
{
    public long EventId { get; set; }
    public long ArtistId { get; set; }
    
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
}