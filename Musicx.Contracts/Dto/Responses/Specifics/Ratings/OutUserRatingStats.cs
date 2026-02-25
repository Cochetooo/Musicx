namespace Musicx.Contracts.Dto.Responses.Specifics.Ratings;

public sealed class OutUserRatingStats
{
    public long UserId { get; set; }

    public Dictionary<int, int> RatingCounts { get; set; } = [];
    public decimal AverageRating { get; set; }
    public decimal RatingStandardDev { get; set; }
    public decimal ArtistNameRatioAMvsNZ { get; set; }
    public short? MostRatedReleaseYear { get; set; }
}