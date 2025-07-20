using Musicx.Domain.Enums;

namespace Musicx.Domain.Models;

public sealed class Album : BaseModel
{
    public Artist? Artist { get; set; }
    public long? ArtistId { get; set; }
    
    public ICollection<Release> Releases { get; set; } = new List<Release>();
    public ICollection<Genre> PrimaryGenres { get; set; } = new List<Genre>();
    public ICollection<Genre> InfluenceGenres { get; set; } = new List<Genre>();
    
    public string? ArtworkUrl { get; set; }
    public int? DiscTotal { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public int? TrackTotal { get; set; }
}