namespace MusicxApi.Models;

public class BandArtist
{
    public List<ulong> MemberIds { get; } = [];
    
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
}