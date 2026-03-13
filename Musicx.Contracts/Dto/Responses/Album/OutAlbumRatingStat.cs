namespace Musicx.Contracts.Dto.Responses.Album;

public sealed class OutAlbumRatingStat
{
    public decimal? Average { get; set; }
    public int Count { get; set; }
    public long Sum { get; set; }
}