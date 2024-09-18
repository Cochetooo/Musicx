using System.Windows.Documents;
using Musicx.Models.Enums;

namespace Musicx.Models;

public class SongEntity
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
    public List<long> GenreIds { get; } = new();
    public List<long> InfluenceGenreIds { get; } = new();
    public List<long> InstrumentIds { get; } = new();
    
    public required AudioFormatType AudioFormat { get; set; }
    public required int BitRate { get; set; }
    public short? DiscNumber { get; set; }
    public required long Duration { get; set; }
    public required string Filepath { get; set; }
    public string? GeneratedGenreName { get; set; }
    public Dictionary<long, string> Lyrics { get; set; } = new();
    public required int SampleRate { get; set; }
    public string? Title { get; set; }
    public short? TrackNumber { get; set; }

    public void SetRawLyrics(string lyrics)
    {
        Lyrics = new Dictionary<long, string>
        {
            { 0L, lyrics }
        };
    }
}