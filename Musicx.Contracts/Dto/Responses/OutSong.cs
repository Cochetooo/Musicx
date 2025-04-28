namespace Musicx.Contracts.Dto.Responses;

public sealed class OutSong : BaseModel
{
    public OutAlbum? Album { get; set; }
    public OutArtist? Artist { get; set; }
    public ICollection<OutGenre> PrimaryGenres { get; set; } = new List<OutGenre>();
    public ICollection<OutGenre> InfluenceGenres { get; set; } = new List<OutGenre>();
    
    public int? DiscNumber { get; set; }
    public long? Duration { get; set; }
    public string? Lyrics { get; set; }
    public string Title { get; set; } = null!;
    public int? TrackNumber { get; set; }
    
    public ushort? BitRate { get; set; }
    public string? FilePath { get; set; } = null!;
    public string? Format { get; set; }
    public double? SampleRate { get; set; }
    public double? VolumeModifier { get; set; }
}