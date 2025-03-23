namespace Musicx.Core.Models;

public class PersonArtist : Artist
{
    public List<ulong> BandIds { get; set; } = [];

    public List<BandArtist> Bands { get; set; } = [];

    public string FirstName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}