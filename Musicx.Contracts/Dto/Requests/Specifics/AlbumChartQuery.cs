using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests.Specifics;

public class AlbumChartQuery
{
    public ChartType ChartType { get; set; } = ChartType.Top;
    public bool DistinctArtists { get; set; }
    public string[]? ExcludedCountries { get; set; }
    public long[]? ExcludedGenres { get; set; }
    public string[]? ExcludedLanguages { get; set; }
    public string[]? IncludedCountries { get; set; }
    public long[]? IncludedGenres { get; set; }
    public string[]? IncludedLanguages { get; set; }
    public short? MaxAge { get; set; }
    public DateTime? MaxDate { get; set; }
    public long? MaxNbRatings { get; set; }
    public decimal? MaxRating { get; set; }
    public short? MinAge { get; set; }
    public DateTime? MinDate { get; set; }
    public long? MinNbRatings { get; set; }
    public decimal? MinRating { get; set; }
    public string? Name { get; set; }
    public short PopularityWeight { get; set; } = 5;
    public ReleaseType[]? ReleaseTypes { get; set; }
    public long Skip { get; set; } = 0;
    public long Take { get; set; } = 100;
    public long[]? UsersIncluded { get; set; }
}