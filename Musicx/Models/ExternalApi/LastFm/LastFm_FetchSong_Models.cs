using Newtonsoft.Json;

namespace Musicx.Models.ExternalApi.LastFm;

public class LastFm_FetchSongObject
{
    [JsonProperty("track")]
    public Track Track { get; set; }
}

public class Track
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
    public Streamable Streamable { get; set; }
    [JsonProperty("listeners")]
    public string Listeners { get; set; }
    [JsonProperty("playcount")]
    public string Playcount { get; set; }
    [JsonProperty("artist")]
    public Artist Artist { get; set; }
    [JsonProperty("album")]
    public Album Album { get; set; }
    [JsonProperty("toptags")]
    public Toptags TopTags { get; set; }
    [JsonProperty("wiki")]
    public Wiki Wiki { get; set; }
}

public class Streamable
{
    [JsonProperty("#text")]
    public string Text { get; set; }
    [JsonProperty("fulltrack")]
    public string FullTrack { get; set; }
}

public class Artist
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("mbid")]
    public string MbId { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
}

public class Album
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
    public Image[] Images { get; set; }
    [JsonProperty("@attr")]
    public _attr _attr { get; set; }
}

public class Image
{
    [JsonProperty("#text")]
    public string Text { get; set; }
    [JsonProperty("size")]
    public string Size { get; set; }
}

public class _attr
{
    [JsonProperty("position")]
    public string Position { get; set; }
}

public class Toptags
{
    [JsonProperty("tag")]
    public Tag[] Tags { get; set; }
}

public class Tag
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
}

public class Wiki
{
    [JsonProperty("published")]
    public string Published { get; set; }
    [JsonProperty("summary")]
    public string Summary { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
}