namespace Musicx.Domain.Models;

public sealed class PersonArtist : Artist
{
    public ICollection<BandArtist> Bands { get; set; } = new List<BandArtist>();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}