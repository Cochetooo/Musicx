using Musicx.Domain.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutSong : BaseOutputModel
{
    public OutAlbum? Album { get; set; }
    public OutArtist? Artist { get; set; }
    public IReadOnlyList<OutGenre> PrimaryGenres { get; set; } = new List<OutGenre>();
    public IReadOnlyList<OutGenre> InfluenceGenres { get; set; } = new List<OutGenre>();
    
    public int? DiscNumber { get; set; }
    public long? Duration { get; set; }
    public string? Lyrics { get; set; }
    public string Title { get; set; } = null!;
    public int? TrackNumber { get; set; }
    public SongType? Type { get; set; }
    
    public ushort? BitRate { get; set; }
    public string? FilePath { get; set; } = null!;
    public AudioFormatType? Format { get; set; }
    public double? SampleRate { get; set; }
    public double? VolumeModifier { get; set; }
}