namespace Musicx.Contracts.Dto.Responses.Album;

public sealed class OutAlbumRatingStat
{
    public decimal? Average { get; set; }
    public int Count { get; set; }
    public long Sum { get; set; }
    public decimal? FanAverage { get; set; }
    public int FanCount { get; set; }
    public decimal? NonFanAverage { get; set; }
    public int NonFanCount { get; set; }
}