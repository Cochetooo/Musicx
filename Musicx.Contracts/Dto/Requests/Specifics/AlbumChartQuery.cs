using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests.Specifics;

public record AlbumChartQuery(   
    ChartType ChartType,
    bool DistinctArtists = false,
    string[]? ExcludedCountries = null,
    long[]? ExcludedGenres = null,
    string[]? ExcludedLanguages = null,
    string[]? IncludedCountries = null,
    long[]? IncludedGenres = null,
    string[]? IncludedLanguages = null,
    short? MaxAge = null,
    DateTime? MaxDate = null,
    long? MaxNbRatings = null,
    decimal? MaxRating = null,
    short? MinAge = null,
    DateTime? MinDate = null,
    long? MinNbRatings = null,
    decimal? MinRating = null,
    string? Name = null,
    short PopularityWeight = 3,
    ReleaseType[]? ReleaseTypes = null,
    long Skip = 0,
    long Take = 100,
    long[]? UsersIncluded = null);