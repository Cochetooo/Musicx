namespace Musicx.Models;

public class BandArtistEntity : ArtistEntity
{
    public List<long> MemberIds { get; } = new();
}