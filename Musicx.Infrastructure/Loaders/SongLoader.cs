using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Loaders;

public interface ISongLoader
{
    Task<Album?> LoadAlbum(ulong? albumId);
    Task<Artist?> LoadArtist(ulong? artistId);
    Task<List<Genre>> LoadGenres(List<ulong> genreIds);
    Task<List<Genre>> LoadInfluenceGenres(List<ulong> genreIds);
}

public class SongLoader(
    IArtistManager artistManager,
    IAlbumManager albumManager,
    IGenreManager genreManager) : ISongLoader
{
    public async Task<Album?> LoadAlbum(ulong? albumId)
    {
        return albumId.HasValue ? await albumManager.FindById(albumId.Value) : null;
    }

    public async Task<Artist?> LoadArtist(ulong? artistId)
    {
        return artistId.HasValue ? await artistManager.FindById(artistId.Value) : null;
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