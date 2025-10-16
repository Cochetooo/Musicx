namespace Musicx.Contracts.Dto.Responses.Specifics.Ratings;

public sealed class OutUserRatingStats
{
    public long UserId { get; set; }

    public Dictionary<int, int> RatingCounts { get; set; } = [];
}