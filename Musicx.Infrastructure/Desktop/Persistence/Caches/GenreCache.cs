using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Caches;

internal sealed class GenreCache(IAppConfiguration appConfiguration) : IGenreCache
{
    private readonly int _maxCacheSize = appConfiguration.GetValue<int>("Persistence.CacheMaxSizes.Genre");
    private readonly Dictionary<string, Genre> _cache = new();

    public Genre? Get(string key) => _cache.TryGetValue(key, out var genre) ? genre : null;

    public void Add(string key, Genre genre)
    {
        if (_cache.Count >= _maxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = genre;
    }
}