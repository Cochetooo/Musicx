namespace Musicx.Core.Models;

public class BandArtist : Artist
{
    public List<ulong> MemberIds { get; set; } = [];
    
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
}