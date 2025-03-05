using Newtonsoft.Json;

namespace Musicx.Models.ExternalApi.LastFm;

public class LastFm_FetchAlbumObject
{
    [JsonProperty("album")]
    public Album Album { get; set; }
}

public class Tags
{
    [JsonProperty("tag")]
    public Tag[] Tag { get; set; }
}

public class Tracks
{
    [JsonProperty("track")]
    public Track[] Track { get; set; }
}

