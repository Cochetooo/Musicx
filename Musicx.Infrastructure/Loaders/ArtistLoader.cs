using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Loaders;

public interface IArtistLoader
{
    Task<List<BandArtist>> LoadBands(List<ulong> artistIds);
    Task<List<PersonArtist>> LoadMembers(List<ulong> artistIds);
}

public class ArtistLoader(IArtistManager artistManager) : IArtistLoader
{
    public async Task<List<PersonArtist>> LoadMembers(List<ulong> artistIds)
    {
        var members = await artistManager.FindIn(artistIds);
        return members.OfType<PersonArtist>().ToList();
    }

    public async Task<List<BandArtist>> LoadBands(List<ulong> artistIds)
    {
        var bands = await artistManager.FindIn(artistIds);
        return bands.OfType<BandArtist>().ToList();
    }
}