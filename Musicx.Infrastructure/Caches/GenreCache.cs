using Musicx.Core.Models;

namespace Musicx.Infrastructure.Caches;

public interface IGenreCache
{
    Genre? Get(string key);
    void Add(string key, Genre genre);
}

public class GenreCache : IGenreCache
{
    private const int MaxCacheSize = 2000;
    private readonly Dictionary<string, Genre> _cache = new();

    public Genre? Get(string key) => _cache.TryGetValue(key, out var genre) ? genre : null;

    public void Add(string key, Genre genre)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = genre;
    }
}