namespace MusicxApi.Models;

public class PersonArtist : Artist
{
    public List<ulong> BandIds { get; set; } = [];
    
    public required string FirstName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}