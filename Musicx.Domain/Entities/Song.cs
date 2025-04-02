using Musicx.Domain.Enums;

namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing a song.
/// </summary>
/// <since>0.3.0</since>
public sealed class Song : BaseEntity
{
    /// <summary>
    /// Indicate the creation date of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    
    /// <summary>
    /// Indicate the last update of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Album where this song appear.
    /// </summary>
    /// <since>0.3.0</since>
    public Album? Album { get; set; }
    
    /// <summary>
    /// Artist that made this song.
    /// </summary>
    /// <since>0.3.0</since>
    public Artist? Artist { get; set; }
    
    /// <summary>
    /// Primary genres of this song, often self-sufficient to describe its mood.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<Genre>? PrimaryGenres { get; set; }
    
    /// <summary>
    /// Noticeable genre influences of this album, giving some precision.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<Genre>? InfluenceGenres { get; set; }
    
    /// <summary>
    /// Album disc number of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public int? DiscNumber { get; set; }
    
    /// <summary>
    /// Length in seconds of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public long? Duration { get; set; }
    
    /// <summary>
    /// Lyrics of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public string? Lyrics { get; set; }
    
    /// <summary>
    /// Name of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Album track number of this song.
    /// </summary>
    /// <since>0.3.0</since>
    public int? TrackNumber { get; set; }
}