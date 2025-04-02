namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing a person artist.
/// </summary>
/// <since>0.3.0</since>
public sealed class PersonArtist : Artist
{
    /// <summary>
    /// Bands in which this person has played.
    /// </summary>
    /// <since>0.3.0</since>
    public ICollection<BandArtist>? Bands { get; set; }
    
    /// <summary>
    /// The first name of the person.
    /// </summary>
    /// <since>0.3.0</since>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// The last name of the person.
    /// </summary>
    /// <since>0.6.1</since>
    public string? LastName { get; set; }
    
    /// <summary>
    /// The date of birth of the person.
    /// </summary>
    /// <since>0.3.0</since>
    public DateTime? BirthDate { get; set; }
    
    /// <summary>
    /// The date of death of the person.
    /// </summary>
    /// <since>0.3.0</since>
    public DateTime? DeathDate { get; set; }
}