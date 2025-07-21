using Musicx.Domain.Enums;

namespace Musicx.Domain.Models;

public sealed class Song : BaseModel
{
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
    public Album? Album { get; set; }
    public Artist? Artist { get; set; }
    public ICollection<Genre> PrimaryGenres { get; set; } = new List<Genre>();
    public ICollection<Genre> InfluenceGenres { get; set; } = new List<Genre>();
    
    public int? DiscNumber { get; set; }
    public long? Duration { get; set; }
    public string? Lyrics { get; set; }
    public string Title { get; set; } = null!;
    public int? TrackNumber { get; set; }
    public SongType? Type { get; set; }
    
    public ushort? BitRate { get; set; }
    public string? FilePath { get; set; }
    public AudioFormatType? Format { get; set; }
    public double? SampleRate { get; set; }
    public double? VolumeModifier { get; set; }
}