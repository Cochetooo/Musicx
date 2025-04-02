namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing an artist. Cannot be created directly as it is either a band or a person.
/// </summary>
/// <since>0.3.0</since>
public abstract class Artist : BaseEntity
{
    /// <summary>
    /// Link as URL to an artwork representing the artist.
    /// </summary>
    /// <since>0.3.0</since>
    public string? ArtworkUrl { get; set; }
    
    /// <summary>
    /// Country of provenance.
    /// </summary>
    /// <since>0.3.0</since>
    public string? Country { get; set; }

    /// <summary>
    /// Name of this artist.
    /// </summary>
    /// <since>0.3.0</since>
    public string Name { get; set; } = string.Empty;
}