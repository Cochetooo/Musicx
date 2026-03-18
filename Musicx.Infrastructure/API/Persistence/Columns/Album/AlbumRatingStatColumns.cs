namespace Musicx.Infrastructure.API.Persistence.Columns.Album;

public static class AlbumRatingStatColumns
{
    public const string AlbumId = "album_rating_stats_album_id";
    
    public const string Average = "album_rating_stats_avg";
    public const string Count = "album_rating_stats_count";
    public const string Sum = "album_rating_stats_sum";
    public const string FanAverage = "album_rating_stats_fan_avg";
    public const string FanCount = "album_rating_stats_fan_count";
    public const string NonFanAverage = "album_rating_stats_non_fan_avg";
    public const string NonFanCount = "album_rating_stats_non_fan_count";
}