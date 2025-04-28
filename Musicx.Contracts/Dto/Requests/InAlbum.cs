namespace Musicx.Contracts.Dto.Requests;

public sealed class InAlbum : BaseModel
{
    public long? ArtistId { get; set; }
    public ICollection<long> ReleaseIds { get; set; } = new List<long>();
    public ICollection<long> PrimaryGenreIds { get; set; } = new List<long>();
    public ICollection<long> InfluenceGenreIds { get; set; } = new List<long>();
    
    public string? ArtworkUrl { get; set; }
    public int? DiscTotal { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    public string? ReleaseType { get; set; }
    public int? TrackTotal { get; set; }
}