namespace Musicx.Contracts.Dto.Requests;

public sealed class InSong : BaseModel
{
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
    public ICollection<long> PrimaryGenreIds { get; set; } = new List<long>();
    public ICollection<long> InfluenceGenreIds { get; set; } = new List<long>();
    
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