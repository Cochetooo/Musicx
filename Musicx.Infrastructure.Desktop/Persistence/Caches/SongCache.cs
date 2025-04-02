using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Domain.Entities;

namespace Musicx.Infrastructure.Desktop.Persistence.Caches;

public class SongCache(IAppConfiguration appConfiguration) : ISongCache
{
    private readonly int _maxCacheSize = appConfiguration.GetValue<int>("Persistence.CacheMaxSizes.Song");
    private readonly Dictionary<string, Song> _cache = new();

    public Song? Get(string key) => _cache.TryGetValue(key, out var song) ? song : null;

    public void Add(string key, Song song)
    {
        if (_cache.Count >= _maxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = song;
    }
}