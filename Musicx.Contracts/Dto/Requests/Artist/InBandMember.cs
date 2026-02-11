namespace Musicx.Contracts.Dto.Requests.Artist;

public sealed class InBandMember : BaseInputModel
{
    public long BandId { get; set; }
    public long MemberId { get; set; }
    
    public ICollection<InBandMemberPeriod>? Periods { get; set; }
    
    public string? Alias { get; set; }
}

public sealed class InBandMemberPeriod
{
    public short? FromYear { get; set; }
    public short? ToYear { get; set; }
}