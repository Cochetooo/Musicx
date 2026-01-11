namespace Musicx.Contracts.Dto.Responses;

public sealed class OutEventArtist : BaseOutputModel
{
    public OutEvent Event { get; set; } = null!;
    public OutArtist Artist { get; set; } = null!;
    
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
}