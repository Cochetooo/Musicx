namespace Musicx.Contracts.Dto.Responses.Artist;

public sealed class OutArtistRatingStat
{
    public decimal? Average { get; set; }
    public long Count { get; set; }
    public decimal? FanAverage { get; set; }
    public long FanCount { get; set; }
    public decimal? NonFanAverage { get; set; }
    public long NonFanCount { get; set; }
}