using Newtonsoft.Json;

namespace Musicx.Core.Models.ExternalApi.LastFm;

public class LastFm_FetchAlbumObject
{
    [JsonProperty("album")]
    public LastFm_Album Album { get; set; }
}

public class LastFm_Tags
{
    [JsonProperty("tag")]
    public LastFm_Tag[] Tag { get; set; }
}

public class LastFm_Tracks
{
    [JsonProperty("track")]
    public LastFm_Track[] Track { get; set; }
}

