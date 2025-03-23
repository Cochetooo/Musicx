using Newtonsoft.Json;

namespace Musicx.Core.Models.ExternalApi.LastFm;

public class LastFm_FetchSongObject
{
    [JsonProperty("track")]
    public LastFm_Track Track { get; set; }
}

public class LastFm_Track
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("mbid")]
    public string MbId { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public string Duration { get; set; }

    [JsonProperty("streamable")]
    public LastFm_Streamable Streamable { get; set; }

    [JsonProperty("listeners")]
    public string Listeners { get; set; }

    [JsonProperty("playcount")]
    public string Playcount { get; set; }

    [JsonProperty("artist")]
    public LastFm_Artist Artist { get; set; }

    [JsonProperty("album")]
    public LastFm_Album Album { get; set; }

    [JsonProperty("toptags")]
    public LastFm_TopTags TopTags { get; set; }

    [JsonProperty("wiki")]
    public LastFm_Wiki Wiki { get; set; }
}

public class LastFm_Streamable
{
    [JsonProperty("#text")]
    public string Text { get; set; }

    [JsonProperty("fulltrack")]
    public string FullTrack { get; set; }
}

public class LastFm_Artist
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("mbid")]
    public string MbId { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

public class LastFm_Album
{
    [JsonProperty("artist")]
    public string Artist { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("mbid")]
    public string MbId { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("image")]
    public LastFm_Image[] Images { get; set; }

    [JsonProperty("@attr")]
    public LastFm_attr _attr { get; set; }
}

public class LastFm_Image
{
    [JsonProperty("#text")]
    public string Text { get; set; }

    [JsonProperty("size")]
    public string Size { get; set; }
}

public class LastFm_attr
{
    [JsonProperty("position")]
    public string Position { get; set; }
}

public class LastFm_TopTags
{
    [JsonProperty("tag")]
    public LastFm_Tag[] Tags { get; set; }
}

public class LastFm_Tag
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

public class LastFm_Wiki
{
    [JsonProperty("published")]
    public string Published { get; set; }

    [JsonProperty("summary")]
    public string Summary { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}