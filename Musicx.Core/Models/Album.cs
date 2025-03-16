using Musicx.Core.Models.Enums;

namespace Musicx.Core.Models;

public class Album
{
    public ulong Id { get; set; }
    
    public ulong? ArtistId { get; set; }
    public Dictionary<ulong, ArtistRole> CreditArtistIds { get; set; } = [];
    public ulong? LabelId { get; set; }
    public List<ulong> GenreIds { get; set; } = [];
    public List<ulong> InfluenceGenreIds { get; set; } = [];
    
    public Artist? Artist { get; set; }
    public Label? Label { get; set; }
    public List<Genre> Genres { get; set; } = [];
    public List<Genre> InfluenceGenres { get; set; } = [];
    
    public string? ArtworkUrl { get; set; }
    public string? CatalogNumber { get; set; }
    public uint? DiscTotal { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public uint? TrackTotal { get; set; }
}