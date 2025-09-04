using System.Text.Json.Serialization;

namespace Musicx.Infrastructure.Shared.Models.ExternalMusicData;

public sealed class LastFmSearchArtist
{
    public record RootObject(
        Artist artist
    );

    public record Artist(
        string name,
        string mbid,
        string url,
        Image[] image,
        string streamable,
        string ontour,
        Stats stats,
        Similar similar,
        Tags tags,
        Bio bio
    );

    public class Image
    {
        [JsonPropertyName("#text")]
        public string _text { get; init; }
        public string size { get; init; }
    };

    public record Stats(
        string listeners,
        string playcount
    );

    public record Similar(
        Artist1[] artist
    );

    public record Artist1(
        string name,
        string url,
        Image1[] image
    );

    public record Image1(
        string _text,
        string size
    );

    public record Tags(
        Tag[] tag
    );

    public record Tag(
        string name,
        string url
    );

    public record Bio(
        Links links,
        string published,
        string summary,
        string content
    );

    public record Links(
        Link link
    );

    public record Link(
        string _text,
        string rel,
        string href
    );

}