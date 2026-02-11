namespace Musicx.Contracts.Dto.Responses.Artist;

public sealed class OutBandMemberPeriod : BaseOutputModel
{
    public OutBandMember BandMember { get; set; } = null!;
    
    public short? FromYear { get; set; }
    public short? ToYear { get; set; }
}