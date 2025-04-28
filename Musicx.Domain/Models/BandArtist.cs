namespace Musicx.Domain.Models;

public sealed class BandArtist : Artist
{
    public ICollection<PersonArtist> Members { get; set; } = new List<PersonArtist>();
    
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
}