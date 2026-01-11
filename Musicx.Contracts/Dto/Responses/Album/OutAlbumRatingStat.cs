namespace Musicx.Contracts.Dto.Responses;

public sealed class OutAlbumRatingStat
{
    public decimal Average { get; set; }
    public int Count { get; set; }
    public long Sum { get; set; }
}