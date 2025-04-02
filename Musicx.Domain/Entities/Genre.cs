namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing a musical genre.
/// </summary>
/// <since>0.3.0</since>
public sealed class Genre : BaseEntity
{
    /// <summary>
    /// Collection of parents genres of this genre.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<Genre>? Parents { get; set; }
    
    /// <summary>
    /// Subgenres from this genre.
    /// </summary>
    /// <since>0.6.0</since>
    public ICollection<Genre>? Children { get; set; }
    
    /// <summary>
    /// Name of this genre.
    /// </summary>
    /// <since>0.3.0</since>
    public string Name { get; set; } = string.Empty;
}