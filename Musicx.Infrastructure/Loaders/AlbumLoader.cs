using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Loaders;

public interface IAlbumLoader
{
    Task<Artist?> LoadArtist(ulong? artistId);
    Task<Label?> LoadLabel(ulong? labelId);
    Task<List<Genre>> LoadGenres(List<ulong> genreIds);
    Task<List<Genre>> LoadInfluenceGenres(List<ulong> influenceGenreIds);
}

public class AlbumLoader(IArtistManager artistManager,
    ILabelManager labelManager,
    IGenreManager genreManager) : IAlbumLoader
{
    public async Task<Artist?> LoadArtist(ulong? artistId)
    {
        return artistId.HasValue ? await artistManager.FindById(artistId.Value) : null;
    }

    public async Task<Label?> LoadLabel(ulong? labelId)
    {
        return labelId.HasValue ? await labelManager.FindById(labelId.Value) : null;
    }

    public async Task<List<Genre>> LoadGenres(List<ulong> genreIds)
    {
        return await genreManager.FindIn(genreIds);
    }

    public async Task<List<Genre>> LoadInfluenceGenres(List<ulong> influenceGenreIds)
    {
        return await genreManager.FindIn(influenceGenreIds);
    }
}