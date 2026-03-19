namespace Musicx.Infrastructure.API.Persistence.Columns.Artist;

public static class ArtistRatingStatColumns
{
    public const string ArtistId = "artist_rating_stats_artist_id";
    
    public const string Average = "artist_rating_stats_avg";
    public const string Count = "artist_rating_stats_count";
    public const string Sum = "artist_rating_stats_sum";
    public const string FanAverage = "artist_rating_stats_fan_avg";
    public const string FanCount = "artist_rating_stats_fan_count";
    public const string NonFanAverage = "artist_rating_stats_non_fan_avg";
    public const string NonFanCount = "artist_rating_stats_non_fan_count";
}