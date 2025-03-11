using System.Diagnostics;
using Musicx.Core.Models;

namespace Musicx.Infrastructure.Caches;

public interface ISongCache
{
    Song? Get(string key);
    void Add(string key, Song song);
}

public class SongCache : ISongCache
{
    private readonly Dictionary<string, Song> _cache = new();
    private const int MaxCacheSize = 10000;
    
    public Song? Get(string key) => _cache.TryGetValue(key, out var song) ? song : null;

    public void Add(string key, Song song)
    {
        if (_cache.Count >= MaxCacheSize)
        {
            _cache.Remove(_cache.Keys.First());
        }

        _cache[key] = song;
    }
}