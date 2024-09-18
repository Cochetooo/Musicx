namespace Musicx.Models;

public class PersonArtistEntity : ArtistEntity
{
    public List<long> BandIds { get; } = new();
    
    public required string FirstName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}