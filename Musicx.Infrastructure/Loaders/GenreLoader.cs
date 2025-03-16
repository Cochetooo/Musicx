using Musicx.Core.Models;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Loaders;

public interface IGenreLoader
{
    Task<List<Genre>> LoadParents(List<ulong> parentIds);
    Task<List<Genre>> LoadChildren(List<ulong> childIds);
}

public class GenreLoader(IGenreManager genreManager) : IGenreLoader
{
    public async Task<List<Genre>> LoadParents(List<ulong> parentIds)
    {
        return await genreManager.FindIn(parentIds);
    }

    public async Task<List<Genre>> LoadChildren(List<ulong> childIds)
    {
        return await genreManager.FindIn(childIds);
    }
}