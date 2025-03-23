namespace Musicx.Core.Models;

public class BandArtist : Artist
{
    public List<ulong> MemberIds { get; set; } = [];

    public List<PersonArtist> Members { get; set; } = [];

    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
}