using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;


namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

internal sealed class GenreCache : IGenreCache
{
    private const int MaxCacheSize = 5000;
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