namespace Musicx.Contracts.Dto.Responses.Specifics.Artists;

public sealed class OutArtistRatingSummary
{
    public decimal? Rating { get; set; }
    public long Count { get; set; }
    public decimal? FanRating { get; set; }
    public long FanCount { get; set; }
    public decimal? NonFanRating { get; set; }
    public long NonFanCount { get; set; }
}