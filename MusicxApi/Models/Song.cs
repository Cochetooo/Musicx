using Musicx.Models.Enums;

namespace MusicxApi.Models;

public class Song
{
    public ulong Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ulong? AlbumId { get; set; }
    public ulong? ArtistId { get; set; }
    public List<ulong> GenreIds { get; } = [];
    public List<ulong> InfluenceGenreIds { get; } = [];
    
    public virtual Album Album { get; set; }
    public virtual Artist Artist { get; set; }
    public virtual List<Genre> Genres { get; set; }
    public virtual List<Genre> InfluenceGenres { get; set; }
    
    public required AudioFormatType AudioFormat { get; set; }
    public required int BitRate { get; set; }
    public uint? DiscNumber { get; set; }
    public required long Duration { get; set; }
    public required string Filepath { get; set; }
    public string? GeneratedGenreName { get; set; }
    public string? Lyrics { get; set; }
    public required int SampleRate { get; set; }
    public string Title { get; set; } = "Untitled";
    public uint? TrackNumber { get; set; }
}