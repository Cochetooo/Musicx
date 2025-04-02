using Musicx.Domain.Enums;

namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing an album.
/// </summary>
/// <since>0.3.0</since>
public sealed class Album : BaseEntity
{
    /// <summary>
    /// Artist of this album.
    /// </summary>
    /// <since>0.3.0</since>
    public Artist? Artist { get; set; }
    
    /// <summary>
    /// Release issues of this album.
    /// </summary>
    /// <since>0.6.1</since>
    public ICollection<Release>? Releases { get; set; }
    
    /// <summary>
    /// Primary genres of this album, often self-sufficient to describe its mood.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<Genre>? PrimaryGenres { get; set; }
    
    /// <summary>
    /// Noticeable genre influences of this album, giving some precision.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<Genre>? InfluenceGenres { get; set; }
    
    /// <summary>
    /// Link as URL to the artwork cover.
    /// </summary>
    /// <since>0.3.0</since>
    public string? ArtworkUrl { get; set; }
    
    /// <summary>
    /// Total number of discs of this album.
    /// </summary>
    /// <since>0.3.0</since>
    public int? DiscTotal { get; set; }

    /// <summary>
    /// Name of this album.
    /// </summary>
    /// <since>0.3.0</since>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Initial date of release for this album.
    /// </summary>
    /// <since>0.3.0</since>
    public DateTime? ReleaseDate { get; set; }
    
    /// <summary>
    /// Release type of this album.
    /// </summary>
    /// <since>0.3.0</since>
    public ReleaseType? ReleaseType { get; set; }
    
    /// <summary>
    /// Total number of tracks of this album.
    /// </summary>
    /// <since>0.3.0</since>
    public int? TrackTotal { get; set; }
}