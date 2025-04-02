namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing a band.
/// </summary>
/// <since>0.3.0</since>
public sealed class BandArtist : Artist
{
    /// <summary>
    /// List of members of this band.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<PersonArtist>? Members { get; set; }
    
    /// <summary>
    /// Formation date of this band.
    /// </summary>
    /// <since>0.6.0</since>
    public DateTime? FormationDate { get; set; }
    
    /// <summary>
    /// Split date of this band.
    /// </summary>
    /// <since>0.6.0</since>
    public DateTime? SplitDate { get; set; }
}