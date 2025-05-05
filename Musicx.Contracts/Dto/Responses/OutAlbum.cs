namespace Musicx.Contracts.Dto.Responses;

public sealed class OutAlbum
{
    public long Id { get; set; }
    
    public OutArtist? Artist { get; set; }
    public ICollection<OutRelease> Releases { get; set; } = new List<OutRelease>();
    public ICollection<OutGenre> PrimaryGenres { get; set; } = new List<OutGenre>();
    public ICollection<OutGenre> InfluenceGenres { get; set; } = new List<OutGenre>();
    
    public string? ArtworkUrl { get; set; }
    public int? DiscTotal { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    public string? ReleaseType { get; set; }
    public int? TrackTotal { get; set; }
}