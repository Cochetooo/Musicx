namespace Musicx.Contracts.Dto.Responses.Artist;

public sealed class OutBandMember : BaseOutputModel
{
    public OutArtist Band { get; set; } = null!;
    public OutArtist Member { get; set; } = null!;
    public ICollection<OutBandMemberPeriod>? ActivityPeriods { get; set; }
    
    public string? Alias { get; set; }
}