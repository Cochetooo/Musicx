using Musicx.Core.Models.Enums;

namespace Musicx.Core.Models;

public class Song
{
    public ulong Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ulong? AlbumId { get; set; }
    public ulong? ArtistId { get; set; }
    public List<ulong> GenreIds { get; set; } = [];
    public List<ulong> InfluenceGenreIds { get; set; } = [];
    
    public AudioFormatType AudioFormat { get; set; } = AudioFormatType.Unknown;
    public int BitRate { get; set; }
    public uint? DiscNumber { get; set; }
    public long Duration { get; set; }
    public string Filepath { get; set; } = string.Empty;
    public string? GeneratedGenreName { get; set; }
    public string? Lyrics { get; set; }
    public int SampleRate { get; set; }
    public string Title { get; set; } = string.Empty;
    public uint? TrackNumber { get; set; }
}