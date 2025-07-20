using System.Text.Json.Serialization;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData.Helpers;

namespace Musicx.Infrastructure.Shared.Models.ExternalMusicData;

public sealed class LastFmSearchAlbum
{
    public class RootObject
{
    public Album album { get; set; }
}

public class Album
{
    public string artist { get; set; }
    public string mbid { get; set; }
    public Tags tags { get; set; }
    public string playcount { get; set; }
    public Image[] image { get; set; }
    public Tracks tracks { get; set; }
    public string url { get; set; }
    public string name { get; set; }
    public string listeners { get; set; }
    public Wiki wiki { get; set; }
}

public class Tags
{
    [JsonConverter(typeof(SingleOrArrayConverter<Tag>))]
    public List<Tag> tag { get; set; }
}

public class Tag
{
    public string url { get; set; }
    public string name { get; set; }
}

public class Image
{
    public string size { get; set; }
    [JsonPropertyName("#text")]
    public string _text { get; set; }
}

public class Tracks
{
    [JsonConverter(typeof(SingleOrArrayConverter<Track>))]
    public List<Track> track { get; set; }
}

public class Track
{
    public Streamable streamable { get; set; }
    public int? duration { get; set; }
    public string url { get; set; }
    public string name { get; set; }
    public _attr _attr { get; set; }
    public Artist artist { get; set; }
}

public class Streamable
{
    public string fulltrack { get; set; }
    public string _text { get; set; }
}

public class _attr
{
    public int rank { get; set; }
}

public class Artist
{
    public string url { get; set; }
    public string name { get; set; }
    public string mbid { get; set; }
}

public class Wiki
{
    public string published { get; set; }
    public string summary { get; set; }
    public string content { get; set; }
}


}