namespace Musicx.Contracts.Dto.Responses.Specifics.Ratings;

public sealed class OutUserGenreRating : BaseOutputModel
{
    public string GenreName { get; set; } = string.Empty;
    public string? GenreColor { get; set; }
    
    public long AlbumCount { get; set; }
    public decimal? WeightedScore { get; set; }
    public decimal? WeightedPercent { get; set; }
}