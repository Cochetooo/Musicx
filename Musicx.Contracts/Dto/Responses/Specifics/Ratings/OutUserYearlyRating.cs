namespace Musicx.Contracts.Dto.Responses.Specifics.Ratings;

public sealed class OutUserYearlyRating
{
    public int YearBucketStart { get; set; }
    public int YearBucketEnd { get; set; }
    public string YearBucketLabel { get; set; } = string.Empty;
    
    public long? GenreId { get; set; }
    public string? GenreName { get; set; }
    public string? GenreColor { get; set; }
    
    public decimal AverageRating { get; set; }
    public long RatingCount { get; set; }
}